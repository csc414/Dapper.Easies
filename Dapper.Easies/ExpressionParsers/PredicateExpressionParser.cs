﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Dapper.Easies
{
    internal class PredicateExpressionParser : ExpressionParser
    {
        private readonly ISqlSyntax _sqlSyntax;

        private QueryContext _context;

        private StringBuilder _sql;

        private ParameterBuilder _parameters;

        private int _binaryDeep = 0;

        internal ParameterBuilder ParameterBuilder => _parameters;

        internal PredicateExpressionParser(ISqlSyntax sqlSyntax, ParameterBuilder parameterBuilder)
        {
            _sqlSyntax = sqlSyntax;
            _parameters = parameterBuilder;
        }

        internal PredicateExpressionParser(ISqlSyntax sqlSyntax, ParameterBuilder parameterBuilder, ReadOnlyCollection<ParameterExpression> parameters) : this(sqlSyntax, parameterBuilder)
        {
            SetParameters(parameters);
        }

        internal string ToSql(Expression exp, QueryContext context)
        {
            if (exp == null)
                return null;

            Clear();
            _context = context;
            _sql = new StringBuilder();
            Visit(exp);
            return _sql.ToString();
        }

        private void Clear()
        {
            _context = null;
            _sql = null;
        }

        internal override ParserData VisitLambda(LambdaExpression lambda)
        {
            AppendSql(Visit(lambda.Body), true);
            return ParserData.Empty;
        }

        internal override ParserData VisitBinary(BinaryExpression b)
        {
            if (b.NodeType == ExpressionType.Coalesce || b.NodeType == ExpressionType.ArrayIndex)
                return CreateConstant(GetValue(b), b);

            PrivateVisit(b.Left);
            if ((b.NodeType == ExpressionType.Equal || b.NodeType == ExpressionType.NotEqual) && b.Right is ConstantExpression constant && constant.Value == null)
            {
                var operatorStr = _sqlSyntax.Operator(b.NodeType == ExpressionType.Equal ? OperatorType.EqualNull : OperatorType.NotEqualNull);
                if (operatorStr == null)
                    throw new NotImplementedException($"ExpressionType：{b.NodeType}");
                _sql.Append(operatorStr);
            }
            else
            {
                var operatorStr = _sqlSyntax.Operator((OperatorType)b.NodeType);
                if (operatorStr == null)
                    throw new NotImplementedException($"ExpressionType：{b.NodeType}");
                _sql.Append(operatorStr);
                PrivateVisit(b.Right);
            }

            void PrivateVisit(Expression exp)
            {
                var condition = b.NodeType != ExpressionType.AndAlso && b.NodeType != ExpressionType.OrElse;
                var hasBracket = (_binaryDeep > 0 && exp is BinaryExpression) || (!condition && exp is BinaryExpression binary && binary.NodeType == ExpressionType.OrElse);

                if (hasBracket)
                    _sql.Append("(");
                if (condition)
                    _binaryDeep++;
                var data = !condition || HasParameter(exp) ? Visit(exp) : CreateConstant(GetValue(exp));
                if (condition)
                    _binaryDeep--;
                AppendSql(data, !condition);
                if (hasBracket)
                    _sql.Append(")");
            }

            return ParserData.Empty;
        }

        internal override ParserData VisitMemberAccess(MemberExpression m)
        {
            var parameterExpression = m.Expression;
            if (parameterExpression?.NodeType == ExpressionType.Convert)
                parameterExpression = ((UnaryExpression)m.Expression).Operand;

            if (parameterExpression is ParameterExpression parameter)
                return CreateProperty(m, parameter);
            else if (parameterExpression is MemberExpression memberExpression && HasParameter(memberExpression.Expression))
            {
                var parserData = VisitMemberAccess(memberExpression);
                if (parserData.Type == ParserDataType.Property && parserData.Value is DbObject.DbProperty dbProperty)
                {
                    if (dbProperty.PropertyInfo.PropertyType.Name == "Nullable`1" && m.Member.Name == "Value")
                        return parserData;
                    else if (dbProperty.PropertyInfo.PropertyType == typeof(DateTime) || dbProperty.PropertyInfo.PropertyType == typeof(DateTime?))
                    {
                        var dateTimeMethod = _sqlSyntax.DateTimeMethod(m.Member.Name, () =>
                        {
                            var aliasIndex = Parameters.IndexOf((ParameterExpression)parserData.Expression);
                            return DbObject.GetTablePropertyAlias(_context, dbProperty, aliasIndex);
                        });
                        if (dateTimeMethod == null)
                            throw new NotImplementedException($"DateTime Method：{m.Member.Name}");

                        return CreateParserData(ParserDataType.Sql, dateTimeMethod);
                    }
                }
            }
            else
            {
                ParserData parserData = null;
                if (m.Expression != null)
                    parserData = Visit(m.Expression);

                if (m.Member is PropertyInfo propertyInfo)
                    return CreateParserData(ParserDataType.Constant, propertyInfo.GetValue(parserData?.Value));

                if (m.Member is FieldInfo fieldInfo)
                    return CreateParserData(ParserDataType.Constant, fieldInfo.GetValue(parserData?.Value));
            }

            throw new NotImplementedException($"ExpressionType：{m.NodeType}");
        }

        internal override ParserData VisitConstant(ConstantExpression c)
        {
            return CreateConstant(c.Value, c);
        }

        internal override ParserData VisitUnary(UnaryExpression u)
        {
            if (u.NodeType == ExpressionType.Convert)
                return Visit(u.Operand);

            if (u.NodeType == ExpressionType.ArrayLength)
            {
                var array = (Array)GetValue(u.Operand);
                return CreateConstant(array.Length, u);
            }

            if (u.NodeType == ExpressionType.Not)
            {
                _sql.Append(_sqlSyntax.Operator(OperatorType.Not));
                _sql.Append("(");
                AppendSql(Visit(u.Operand), true);
                _sql.Append(")");
            }

            return ParserData.Empty;
        }

        internal override ParserData VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.IsStatic)
            {
                if (_dbObjectExtensions == m.Method.ReflectedType)
                {
                    if (m.Method.Name.Equals("Property", StringComparison.Ordinal))
                    {
                        var parameterExpression = m.Arguments[0];
                        if (parameterExpression.NodeType == ExpressionType.Convert)
                            parameterExpression = ((UnaryExpression)parameterExpression).Operand;

                        if (parameterExpression is ParameterExpression parameter)
                        {
                            var name = GetValue(m.Arguments[1]);
                            return CreateParserData(ParserDataType.Property, DbObject.Get(parameter.Type)[name?.ToString()], m);
                        }
                    }
                }
                else if (_dbFuncType.IsAssignableFrom(m.Method.ReflectedType))
                    return CreateSql(GetExpression(m, _parameters, _sqlSyntax, _context, Parameters));
            }

            return CreateConstant(GetValue(m), m);
        }

        void AppendSql(ParserData data, bool andAlso = false)
        {
            switch (data.Type)
            {
                case ParserDataType.Property:
                    {
                        var aliasIndex = Parameters.IndexOf((ParameterExpression)data.Expression);
                        PropertyInfo propertyInfo = null;
                        if (data.Value is DbObject.DbProperty property)
                        {
                            _sql.Append(DbObject.GetTablePropertyAlias(_context, property, aliasIndex));
                            propertyInfo = property.PropertyInfo;
                        }
                        else if (data.Value is MemberExpression memberExpression)
                        {
                            _sql.Append(string.Format("{0}.{1}", _context.Alias[aliasIndex].Alias, _sqlSyntax.EscapePropertyName(memberExpression.Member.Name)));
                            propertyInfo = (PropertyInfo)memberExpression.Member;
                        }
                        if (andAlso && propertyInfo?.PropertyType == typeof(bool))
                            _sql.AppendFormat("{0}{1}", _sqlSyntax.Operator(OperatorType.Equal), _parameters.AddParameter(true));
                    }
                    break;
                case ParserDataType.Constant:
                    _sql.Append(_parameters.AddParameter(data.Value));
                    break;
                case ParserDataType.Sql:
                    _sql.Append(data.Value);
                    break;
            }
        }
    }
}
