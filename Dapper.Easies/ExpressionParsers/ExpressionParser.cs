using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Dapper.Easies
{
    internal abstract class ExpressionParser
    {
        protected static Type _dbFuncType = typeof(DbFunc);

        protected static Type _dbObjectExtensions = typeof(DbObjectExtensions);

        protected LambdaExpression Lambda => _lambda;

        private LambdaExpression _lambda;

        private int _deep = 0;

        internal ParserData Visit(Expression exp)
        {
            ParserData data;
            _deep++;
            switch (exp.NodeType)
            {
                case ExpressionType.Lambda:
                    data = VisitLambda(_lambda = (LambdaExpression)exp);
                    break;
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.Power:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                    data = VisitBinary((BinaryExpression)exp);
                    break;
                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                case ExpressionType.Not:
                    data = VisitUnary((UnaryExpression)exp);
                    break;
                case ExpressionType.MemberAccess:
                    data = VisitMemberAccess((MemberExpression)exp);
                    break;
                case ExpressionType.Constant:
                    data = VisitConstant((ConstantExpression)exp);
                    break;
                case ExpressionType.Call:
                    data = VisitMethodCall((MethodCallExpression)exp);
                    break;
                default:
                    throw new NotImplementedException($"{exp.NodeType}");
            }
            _deep--;
            return data;
        }

        internal virtual ParserData VisitLambda(LambdaExpression lambda)
        {
            Visit(lambda.Body);
            return ParserData.Empty;
        }

        internal virtual ParserData VisitBinary(BinaryExpression b)
        {
            return ParserData.Empty;
        }

        internal virtual ParserData VisitUnary(UnaryExpression u)
        {
            return ParserData.Empty;
        }

        internal virtual ParserData VisitMethodCall(MethodCallExpression m)
        {
            return ParserData.Empty;
        }

        internal virtual ParserData VisitMemberAccess(MemberExpression m)
        {
            return ParserData.Empty;
        }

        internal virtual ParserData VisitConstant(ConstantExpression c)
        {
            return ParserData.Empty;
        }

        protected ParserData CreateSql(string sql)
        {
            return CreateSql(sql, null);
        }

        protected ParserData CreateSql(string sql, Expression expression)
        {
            return CreateParserData(ParserDataType.Sql, sql, expression);
        }

        protected ParserData CreateConstant(object value)
        {
            return CreateConstant(value, null);
        }

        protected ParserData CreateConstant(object value, Expression expression)
        {
            return CreateParserData(ParserDataType.Constant, value, expression);
        }

        protected ParserData CreateParserData(ParserDataType type, object value)
        {
            return CreateParserData(type, value, null);
        }

        protected ParserData CreateParserData(ParserDataType type, object value, Expression expression)
        {
            return new ParserData(type, value, expression) { Deep = _deep };
        }

        protected int GetDeep() => _deep;

        internal static string GetExpression(Expression expression, ParameterBuilder builder, ISqlSyntax sqlSyntax, QueryContext context, ReadOnlyCollection<ParameterExpression> parameters)
        {
            if (expression == null)
                return null;

            if (expression.NodeType == ExpressionType.Lambda)
                return GetExpression(((LambdaExpression)expression).Body, builder, sqlSyntax, context, parameters);

            if (expression.NodeType == ExpressionType.Convert)
                return GetExpression(((UnaryExpression)expression).Operand, builder, sqlSyntax, context, parameters);

            if (expression.NodeType == ExpressionType.Call)
            {
                var m = (MethodCallExpression)expression;
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
                                var name = GetValue(m.Arguments[1])?.ToString();

                                var table = DbObject.Get(parameter.Type);
                                if (table == null)
                                    return name;

                                string alias = null;
                                if (context != null)
                                    alias = $"{context.Alias[parameters.IndexOf(parameter)].Alias}.";

                                return $"{alias}{table[name].EscapeName}";
                            }
                        }
                    }
                    else if (_dbFuncType.IsAssignableFrom(m.Method.ReflectedType))
                    {
                        if (m.Method.Name.Equals("Expr", StringComparison.Ordinal))
                        {
                            var arg = m.Arguments[0];
                            if (arg.NodeType == ExpressionType.Call && arg is MethodCallExpression mm && mm.Method.Name.Equals("Format", StringComparison.Ordinal))
                            {
                                var args = mm.Arguments.Select((o, j) =>
                                {
                                    if (j == 0)
                                        return GetValue(o);

                                    if (o.NodeType == ExpressionType.NewArrayInit)
                                    {
                                        var newArrayExpression = (NewArrayExpression)o;
                                        var args = newArrayExpression.Expressions.Select(e => GetExpression(e, builder, sqlSyntax, context, parameters)).ToArray();
                                        var ary = (object[])Activator.CreateInstance(newArrayExpression.Type, args.Length);
                                        for (int i = 0; i < ary.Length; ++i)
                                            ary[i] = args[i];
                                        return ary;
                                    }

                                    return GetExpression(o, builder, sqlSyntax, context, parameters);
                                }).ToArray();

                                return mm.Method.Invoke(null, args).ToString();
                            }
                            else
                                return GetValue(arg).ToString();
                        }
                        else
                        {
                            var result = sqlSyntax.Method(m.Method, m.Arguments.ToArray(), builder, exp => exp == null ? null : GetExpression(exp, builder, sqlSyntax, context, parameters), exp => exp == null ? null : GetValue(exp));
                            if (result == null)
                                throw new NotImplementedException($"MethodName：{m.Method.Name}");

                            return result;
                        }
                    }
                }
            }

            if (expression is MemberExpression memberExpression)
            {
                if (memberExpression.Expression is ParameterExpression parameter)
                {
                    var table = DbObject.Get(parameter.Type);
                    if (table == null)
                        return memberExpression.Member.Name;
                    
                    string alias = null;
                    if (context != null)
                        alias = $"{context.Alias[parameters.IndexOf(parameter)].Alias}.";

                    return $"{alias}{table[memberExpression.Member.Name].EscapeName}";
                }
                else if(memberExpression.Expression is MemberExpression m && HasParameter(m.Expression))
                {
                    if (memberExpression.Member.Name != "Value" && memberExpression.Member.ReflectedType.Name != "Nullable`1")
                    {
                        if (m.Member.Name == "Value" && m.Member.ReflectedType.Name == "Nullable`1")
                            m = (MemberExpression)m.Expression;

                        if (m.Type == typeof(DateTime) || m.Type == typeof(DateTime?))
                        {
                            var dateTimeMethod = sqlSyntax.DateTimeMethod(memberExpression.Member.Name, () => GetExpression(m, builder, sqlSyntax, context, parameters));
                            if (dateTimeMethod == null)
                                throw new NotImplementedException($"DateTime Method：{memberExpression.Member.Name}");

                            return dateTimeMethod;
                        }
                    }
                    
                    return GetExpression(m, builder, sqlSyntax, context, parameters);
                }
            }

            return builder.AddParameter(GetValue(expression));
        }

        internal static bool HasParameter(Expression exp)
        {
            if (exp == null)
                return false;

            switch (exp.NodeType)
            {
                case ExpressionType.Lambda:
                    return HasParameter(((LambdaExpression)exp).Body);
                case ExpressionType.Parameter:
                    return true;
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.Power:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                    var binary = (BinaryExpression)exp;
                    if (HasParameter(binary.Left))
                        return true;

                    if (HasParameter(binary.Right))
                        return true;
                    break;
                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                case ExpressionType.Not:
                    var unary = (UnaryExpression)exp;
                    if (HasParameter(unary.Operand))
                        return true;
                    break;
                case ExpressionType.MemberAccess:
                    var member = (MemberExpression)exp;
                    if (member.Expression != null && HasParameter(member.Expression))
                        return true;
                    break;
                case ExpressionType.Call:
                    var methodCall = (MethodCallExpression)exp;
                    if (_dbFuncType.IsAssignableFrom(methodCall.Method.ReflectedType))
                        return true;

                    if (methodCall.Object != null && HasParameter(methodCall.Object))
                        return true;

                    foreach (var arg in methodCall.Arguments)
                    {
                        if (HasParameter(arg))
                            return true;
                    }
                    break;
            }

            return false;
        }

        internal static object GetValue(Expression expression)
        {
            if (expression == null)
                return null;

            if (expression.NodeType == ExpressionType.Constant)
                return ((ConstantExpression)expression).Value;

            if (expression.NodeType == ExpressionType.Convert)
                return GetValue(((UnaryExpression)expression).Operand);

            return Expression.Lambda(expression).Compile().DynamicInvoke();
        }
    }

    public enum ParserDataType
    {
        Empty,
        Property,
        Constant,
        Sql
    }

    public class ParserData
    {
        public static ParserData Empty = new ParserData(ParserDataType.Empty, null);

        public ParserData(ParserDataType type, object value)
        {
            Type = type;
            Value = value;
        }

        public ParserData(ParserDataType type, object value, Expression expression) : this(type, value)
        {
            Expression = expression;
        }

        public int Deep { get; internal set; }

        public Expression Expression { get; }

        public ParserDataType Type { get; }

        public object Value { get; }
    }
}
