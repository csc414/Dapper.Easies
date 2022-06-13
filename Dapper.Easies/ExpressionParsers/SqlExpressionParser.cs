using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Dapper.Easies
{
    public class SqlExpressionParser : ExpressionParser
    {
        private StringBuilder _sb = null;

        private ParameterBuilder _parameters = null;

        protected StringBuilder Builder => _sb;

        protected ParameterBuilder Parameters => _parameters;

        private readonly QueryContext _context;

        public SqlExpressionParser(QueryContext context)
        {
            _context = context;
        }

        public virtual void Visit(Expression node, StringBuilder sb, ParameterBuilder parameters)
        {
            _sb = sb;
            _parameters = parameters;
            Visit(node);
            _sb = null;
            _parameters = null;
        }

        public virtual void Visit(IEnumerable<Expression> nodes, StringBuilder sb, ParameterBuilder parameters)
        {
            _sb = sb;
            _parameters = parameters;
            bool hasLoop = false;
            foreach (var node in nodes)
            {
                if (hasLoop)
                    AppendLogicalOperator(ExpressionType.AndAlso);
                Builder.Append("(");
                Visit(node);
                Builder.Append(")");
                hasLoop = true;
            }
            _sb = null;
            _parameters = null;
        }

        public virtual void VisitFields(Expression node, StringBuilder sb, ParameterBuilder parameters, string separator = ", ", bool hasAlias = true)
        {
            var lambda = (LambdaExpression)node;
            SetLambdaExpression(lambda);
            node = lambda.Body;
            _sb = sb;
            _parameters = parameters;
            if (node is MemberInitExpression memberInitExp)
                VisitMemberInit(memberInitExp, separator, hasAlias);
            else if (node is NewExpression newExp)
                VisitNew(newExp, separator, hasAlias);
            else
            {
                node = Visit(node);
                if(node is MemberExpression memberExp)
                    Builder.Append(GetPropertyName(memberExp));
            }
            _sb = null;
            _parameters = null;
            SetLambdaExpression(null);
        }

        protected Expression VisitMemberInit(MemberInitExpression node, string separator, bool hasAlias = true)
        {
            var sqlSyntax = _context.DbObject.SqlSyntax;
            for (int i = 0; i < node.Bindings.Count; i++)
            {
                if (i > 0)
                    Builder.Append(separator);

                if (node.Bindings[i] is MemberAssignment assignment)
                {
                    var arg = Visit(assignment.Expression);
                    if (arg is MemberExpression member)
                    {
                        if(hasAlias)
                            Builder.Append(sqlSyntax.AliasPropertyName(GetPropertyName(member), assignment.Member.Name));
                        else
                            Builder.Append(GetPropertyName(member));
                    }
                    else if (hasAlias && arg is SqlExpression sql)
                        Builder.Append(sqlSyntax.AliasPropertyName($"({sql.Sql})", assignment.Member.Name));
                }
            }
            return node;
        }

        protected NewExpression VisitNew(NewExpression node, string separator, bool hasAlias = true)
        {
            var sqlSyntax = _context.DbObject.SqlSyntax;
            for (int i = 0; i < node.Members.Count; i++)
            {
                if (i > 0)
                    Builder.Append(separator);
                var member = node.Members[i];
                var arg = Visit(node.Arguments[i]);
                if (arg is MemberExpression memberExp)
                {
                    if (hasAlias)
                        Builder.Append(sqlSyntax.AliasPropertyName(GetPropertyName(memberExp), member.Name));
                    else
                        Builder.Append(GetPropertyName(memberExp));

                    
                }
                else if (hasAlias && arg is SqlExpression sql)
                    Builder.Append(sqlSyntax.AliasPropertyName($"({sql.Sql})", member.Name));
            }
            return node;
        }

        protected override string GetPropertyName(Expression exp)
        {
            if (exp is MemberExpression member)
            {
                var parameter = (ParameterExpression)member.Expression;
                var table = DbObject.Get(parameter.Type);
                var aliasIndex = Lambda.Parameters.IndexOf(parameter);
                var alias = _context.Alias[aliasIndex].Alias;
                if (table == null)
                    return $"{alias}.{table.SqlSyntax.EscapePropertyName(member.Member.Name)}";

                return $"{alias}.{table[member.Member.Name].EscapeName}";
            }
            else if (exp is SqlExpression sql)
                return sql.ToString();

            throw new NotSupportedException($"{exp}");
        }

        protected override string GetParameterName(object value)
        {
            return Parameters.Add(value);
        }

        protected override Expression AppendMethod(Expression instance, MethodInfo method, Expression[] args)
        {
            if (instance == null)
            {
                switch (method.Name)
                {
                    case "Property":
                        var parameter = (ParameterExpression)args[0];
                        var name = GetConstantValue<string>(args[1]);
                        var property = parameter.Type.GetProperty(name);
                        if (property == null)
                            throw new ArgumentException($"{parameter.Name}.{name} property not found");
                        return Expression.MakeMemberAccess(args[0], property);
                    case "Expr":
                        if (args[0] is ExprExpression sql)
                            return CreateSql(sql.Sql);
                        else if (args[0] is ConstantExpression constant)
                            return CreateSql(constant.Value?.ToString());
                        else
                            throw new NotSupportedException($"{method.ReflectedType.Name}.{method.Name} passing {args[0]} not supported");
                    case "Like":
                        return CreateSql($"{GetPropertyName(args[0])} LIKE {Parameters.Add(GetConstantValue(args[1]))}");
                    case "In":
                        return CreateSql($"{GetPropertyName(args[0])} IN {Parameters.Add(GetConstantValue(args[1]))}");
                    case "NotIn":
                        return CreateSql($"{GetPropertyName(args[0])} NOT IN {Parameters.Add(GetConstantValue(args[1]))}");
                    case "Count":
                        return CreateSql($"COUNT({(args.Length > 0 ? GetPropertyName(args[0]) : "*")})");
                    case "Min":
                        return CreateSql($"MIN({GetPropertyName(args[0])})");
                    case "Max":
                        return CreateSql($"MAX({GetPropertyName(args[0])})");
                    case "Avg":
                        return CreateSql($"AVG({GetPropertyName(args[0])})");
                    case "Sum":
                        return CreateSql($"SUM({GetPropertyName(args[0])})");
                    case "IsNull":
                        return CreateSql($"{GetPropertyName(args[0])} IS NULL");
                    case "IsNotNull":
                        return CreateSql($"{GetPropertyName(args[0])} IS NOT NULL");
                    case "N":
                        return args[0];
                    default:
                        throw new NotSupportedException($"{method.ReflectedType.Name}.{method.Name} not supported");
                }
            }
            else
            {
                switch (method.Name)
                {
                    case "EndsWith":
                        return CreateSql($"{GetPropertyName(instance)} LIKE {Parameters.Add($"%{GetConstantValue(args[0])}")}");
                    case "StartsWith":
                        return CreateSql($"{GetPropertyName(instance)} LIKE {Parameters.Add($"{GetConstantValue(args[0])}%")}");
                    case "Contains":
                        return CreateSql($"{GetPropertyName(instance)} LIKE {Parameters.Add($"%{GetConstantValue(args[0])}%")}");
                    default:
                        throw new NotSupportedException($"{method.ReflectedType.Name}.{method.Name} not supported");
                }
            }
        }

        protected override void AppendLogicalOperator(ExpressionType type)
        {
            if (type == ExpressionType.AndAlso)
                Builder.Append(" AND ");
            else
                Builder.Append(" OR ");
        }

        protected override Expression AppendDateTimeMember(Expression instance, MemberInfo member)
        {
            switch (member.Name)
            {
                case "Year":
                    return new SqlExpression($"YEAR({GetPropertyName(instance)}))");
                case "Month":
                    return new SqlExpression($"MONTH({GetPropertyName(instance)}))");
                case "Day":
                    return new SqlExpression($"DAY({GetPropertyName(instance)}))");
                default:
                    throw new NotSupportedException($"{GetPropertyName(instance)}.{member.Name} not supported");
            }
        }

        protected override void AppendPredicate(Expression left, Expression right, ExpressionType type)
        {
            Handle(left);

            switch (type)
            {
                case ExpressionType.Equal:
                    if (right == null)
                        Builder.Append(" IS NULL");
                    else
                        Builder.Append(" = ");
                    break;
                case ExpressionType.NotEqual:
                    if (right == null)
                        Builder.Append(" IS NOT NULL");
                    else
                        Builder.Append(" <> ");
                    break;
                case ExpressionType.GreaterThan:
                    Builder.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    Builder.Append(" >= ");
                    break;
                case ExpressionType.LessThan:
                    Builder.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    Builder.Append(" <= ");
                    break;
            }

            Handle(right);

            void Handle(Expression exp)
            {
                if (exp != null)
                {
                    if (exp is ConstantExpression constant)
                        Builder.Append(Parameters.Add(constant.Value));
                    else if (exp is MemberExpression member)
                        Builder.Append(GetPropertyName(member));
                    else if (exp is SqlExpression sql)
                        Builder.Append(sql.Sql);
                    else
                        throw new NotSupportedException($"{exp.GetType().Name} not supported");
                }
            }
        }

        protected override Expression AppendCalculate(Expression left, Expression right, ExpressionType type)
        {
            var leftStr = GetString(left);
            var rightStr = GetString(right);

            switch (type)
            {
                case ExpressionType.Add:
                    return new SqlExpression($"({leftStr} + {rightStr})");
                case ExpressionType.Subtract:
                    return new SqlExpression($"({leftStr} - {rightStr})");
                case ExpressionType.Multiply:
                    return new SqlExpression($"({leftStr} * {rightStr})");
                case ExpressionType.Divide:
                    return new SqlExpression($"({leftStr} / {rightStr})");
            }

            return null;

            string GetString(Expression exp)
            {
                if (exp is ConstantExpression constant)
                    return Parameters.Add(constant.Value);
                else if (exp is MemberExpression member)
                    return GetPropertyName(member);
                else if (exp is SqlExpression sql)
                    return sql.Sql;
                else
                    throw new NotSupportedException($"{exp.GetType().Name} not supported");
            }
        }

        protected override void AppendSql(string sql)
        {
            Builder.Append(sql);
        }

        protected override Expression AppendNot(SqlExpression sql)
        {
            return CreateSql($"NOT({sql.Sql})");
        }
    }
}
