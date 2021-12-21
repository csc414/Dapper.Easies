using System;
using System.Collections.Generic;
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

        internal PredicateExpressionParser(ISqlSyntax sqlSyntax, ParameterBuilder parameterBuilder)
        {
            _sqlSyntax = sqlSyntax;
            _parameters = parameterBuilder;
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
            AppendSql(Visit(lambda.Body));
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
                var hasBracket = _binaryDeep > 0 && exp is BinaryExpression;
                if (hasBracket)
                    _sql.Append("(");
                if (condition)
                    _binaryDeep++;
                var data = Visit(exp);
                AppendSql(data);
                if (condition)
                    _binaryDeep--;
                if (hasBracket)
                    _sql.Append(")");
            }

            return ParserData.Empty;
        }

        internal override ParserData VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression is ParameterExpression)
                return CreateParserData(ParserDataType.Property, DbObject.Get(m.Member.ReflectedType)[m.Member.Name], m);
            else
            {
                var parserData = Visit(m.Expression);
                if (parserData.Type == ParserDataType.Constant)
                {
                    if (m.Member is PropertyInfo propertyInfo)
                        return CreateParserData(ParserDataType.Constant, propertyInfo.GetValue(parserData.Value));

                    if (m.Member is FieldInfo fieldInfo)
                        return CreateParserData(ParserDataType.Constant, fieldInfo.GetValue(parserData.Value));
                }
            }

            return ParserData.Empty;
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
                AppendSql(Visit(u.Operand));
                _sql.Append(")");
            }

            return ParserData.Empty;
        }

        internal override ParserData VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.IsStatic && typeof(DbFunction).IsAssignableFrom(m.Method.ReflectedType))
            {
                var data = Visit(m.Arguments[0]);
                if (data.Type == ParserDataType.Property)
                {
                    var property = (DbObject.DbProperty)data.Value;
                    var args = m.Arguments.Skip(1).Select(o => GetValue(o));
                    var result = _sqlSyntax.Method(m.Method, GetTablePropertyAlias(property), args.ToArray(), _parameters);
                    if (result == null)
                        throw new NotImplementedException($"MethodName：{m.Method.Name}");
                    _sql.Append(result);
                    return ParserData.Empty;
                }
                else
                    throw new ArgumentException("自定义方法第一个字段必须是实体参数");
            }

            return CreateConstant(GetValue(m), m);
        }

        void AppendSql(ParserData data)
        {
            switch (data.Type)
            {
                case ParserDataType.Property:
                    {
                        var property = (DbObject.DbProperty)data.Value;
                        _sql.Append(GetTablePropertyAlias(property));
                    }
                    break;
                case ParserDataType.Constant:
                    _sql.Append(_parameters.AddParameter(data.Value));
                    break;
            }
        }

        string GetTablePropertyAlias(DbObject.DbProperty property)
        {
            return string.Format("{0}.{1}", _context.Alias[property.PropertyInfo.ReflectedType].Alias, property.EscapeName);
        }
    }
}
