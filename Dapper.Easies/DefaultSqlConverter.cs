using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using static Dapper.SqlMapper;

namespace Dapper.Easies
{
    internal class DefaultSqlConverter : ISqlConverter
    {
        private readonly EasiesOptions _options;

        private readonly ISqlSyntax _sqlSyntax;

        private readonly ILogger _logger;

        public DefaultSqlConverter(EasiesOptions options, ISqlSyntax sqlSyntax, ILoggerFactory factory = null)
        {
            _options = options;
            _sqlSyntax = sqlSyntax;
            _logger = factory?.CreateLogger<DefaultSqlConverter>();
            DbObject.Initialize(sqlSyntax);
        }

        public string ToQuerySql(QueryContext context, out DynamicParameters parameters, int? take = null, AggregateInfo aggregateInfo = null)
        {
            var parameterBuilder = new ParameterBuilder(_sqlSyntax);
            var parser = new PredicateExpressionParser(_sqlSyntax, parameterBuilder);
            var sql = _sqlSyntax.SelectFormat(
                _sqlSyntax.TableNameAlias(context.Alias.Values.First()),
                GetFields(context, aggregateInfo), GetJoins(context, parser),
                GetPredicate(context, parser),
                aggregateInfo == null ? GetOrderBy(context) : null,
                context.Skip,
                take ?? context.Take);
            parameters = parameterBuilder.GetDynamicParameters();
            if (_options.DevelopmentMode)
                _logger?.LogParametersSql(sql, parameters);
            return sql;
        }

        public string ToGetSql<T>(object[] ids, out DynamicParameters parameters)
        {
            var table = DbObject.Get(typeof(T));
            var primaryKeys = table.Properties.Where(o => o.PrimaryKey).ToArray();
            if (primaryKeys.Length == 0)
                throw new ArgumentException("获取失败，实体类没有主键");

            var dynamicParameters = new DynamicParameters();
            var sql = _sqlSyntax.SelectFormat(
                table.EscapeName,
                table.Properties.Select(o => o.EscapeNameAsAlias),
                null,
                string.Join(" and ", primaryKeys.Select((o, i) =>
                {
                    var name = _sqlSyntax.ParameterName(o.PropertyInfo.Name);
                    dynamicParameters.Add(name, ids[i]);
                    return $"{o.EscapeName} = {name}";
                })),
                null,
                0,
                1);
            parameters = dynamicParameters;
            if (_options.DevelopmentMode)
                _logger?.LogParametersSql(sql, parameters);
            return sql;
        }

        public string ToInsertSql<T>(T entity, out DynamicParameters parameters, out bool hasIdentityKey)
        {
            var table = DbObject.Get(typeof(T));
            hasIdentityKey = table.IdentityKey != null;
            var properties = table.Properties.Where(o => !o.IdentityKey);
            var sql = _sqlSyntax.InsertFormat(
                table.EscapeName,
                properties.Select(o => o.EscapeName),
                properties.Select(o => _sqlSyntax.ParameterName(o.PropertyInfo.Name)),
                hasIdentityKey);
            parameters = new DynamicParameters(entity);
            if (_options.DevelopmentMode)
                _logger?.LogSql(sql);
            return sql;
        }

        public string ToDeleteSql(QueryContext context, bool correlation, out DynamicParameters parameters)
        {
            var parameterBuilder = new ParameterBuilder(_sqlSyntax);
            var parser = new PredicateExpressionParser(_sqlSyntax, parameterBuilder);
            var deleteTableAlias = context.Alias.Select(o => o.Value.Alias);
            if (!correlation)
                deleteTableAlias = deleteTableAlias.Take(1);
            var sql = _sqlSyntax.DeleteFormat(
                _sqlSyntax.TableNameAlias(context.Alias.Values.First()),
                deleteTableAlias,
                GetJoins(context, parser),
                GetPredicate(context, parser));
            parameters = parameterBuilder.GetDynamicParameters();
            if (_options.DevelopmentMode)
                _logger?.LogParametersSql(sql, parameters);
            return sql;
        }

        public string ToUpdateFieldsSql(Expression updateFieldsExp, QueryContext context, out DynamicParameters parameters)
        {
            var lambda = (LambdaExpression)updateFieldsExp;
            if (!(lambda.Body is MemberInitExpression initExp))
                throw new NotImplementedException($"NodeType：{lambda.Body.NodeType}");

            var parameterBuilder = new ParameterBuilder(_sqlSyntax);
            var parser = new PredicateExpressionParser(_sqlSyntax, parameterBuilder);
            var fields = new List<string>();
            foreach (var binding in initExp.Bindings)
            {
                if (binding is MemberAssignment assignment)
                {
                    var value = ExpressionParser.GetValue(assignment.Expression);
                    var table = DbObject.Get(binding.Member.ReflectedType);
                    var alias = context.Alias[table.Type];
                    fields.Add($"{alias.Alias}.{table[binding.Member.Name].EscapeName} = {parameterBuilder.AddParameter(value)}");
                }
                else
                    throw new NotImplementedException($"BindingType：{binding.BindingType}");
            }

            var sql = _sqlSyntax.UpdateFormat(
                _sqlSyntax.TableNameAlias(context.Alias.Values.First()),
                fields,
                GetPredicate(context, parser));
            parameters = parameterBuilder.GetDynamicParameters();
            if (_options.DevelopmentMode)
                _logger?.LogParametersSql(sql, parameters);
            return sql;
        }

        public string ToUpdateSql<T>(T entity, out DynamicParameters parameters)
        {
            var table = DbObject.Get(typeof(T));
            var primaryKeys = table.Properties.Where(o => o.PrimaryKey).ToArray();
            if (primaryKeys.Length == 0)
                throw new ArgumentException("更新失败，实体类没有主键");

            var properties = table.Properties.Where(o => !o.PrimaryKey);
            var sql = _sqlSyntax.UpdateFormat(
                table.EscapeName,
                properties.Select(o => $"{o.EscapeName} = {_sqlSyntax.ParameterName(o.PropertyInfo.Name)}"),
                string.Join(" and ", primaryKeys.Select(o => $"{o.EscapeName} = {_sqlSyntax.ParameterName(o.PropertyInfo.Name)}")));
            parameters = new DynamicParameters(entity);
            if (_options.DevelopmentMode)
                _logger?.LogSql(sql);
            return sql;
        }

        string GetPredicate(QueryContext context, PredicateExpressionParser parser)
        {
            if (context.WhereExpressions.Count > 0)
                return string.Join(_sqlSyntax.Operator(OperatorType.AndAlso), context.WhereExpressions.Select(o => parser.ToSql(o, context)));
            return null;
        }

        IEnumerable<string> GetFields(QueryContext context, AggregateInfo aggregateInfo)
        {
            if (aggregateInfo != null)
            {
                switch (aggregateInfo.Type)
                {
                    case AggregateType.Count:
                        var field = "*";
                        if (aggregateInfo.Expression != null)
                            field = GetPropertyName(aggregateInfo.Expression, context);
                        return new[] { $"count({field})" };
                    case AggregateType.Max:
                        return new[] { $"max({GetPropertyName(aggregateInfo.Expression, context)})" };
                    case AggregateType.Min:
                        return new[] { $"min({ GetPropertyName(aggregateInfo.Expression, context)})" };
                    default:
                        throw new NotImplementedException($"AggregateType：{aggregateInfo.Type}");
                }
            }

            if (context.SelectorExpression != null)
            {
                var fields = new List<string>();
                var lambda = (LambdaExpression)context.SelectorExpression;
                if (lambda.Body is MemberInitExpression initExp)
                {
                    foreach (var binding in initExp.Bindings)
                    {
                        if (binding is MemberAssignment assignment)
                        {
                            if (assignment.Expression.NodeType != ExpressionType.MemberAccess)
                                throw new NotImplementedException($"NodeType：{assignment.Expression.NodeType}");

                            var member = (MemberExpression)assignment.Expression;
                            if (member.Expression.NodeType != ExpressionType.Parameter)
                                throw new NotImplementedException($"BindingType：{member.Expression.NodeType}");

                            var table = DbObject.Get(member.Member.ReflectedType);
                            var alias = context.Alias[table.Type];
                            fields.Add($"{alias.Alias}.{_sqlSyntax.PropertyNameAlias(new DbAlias(table[member.Member.Name].DbName, assignment.Member.Name))}");
                        }
                        else
                            throw new NotImplementedException($"BindingType：{binding.BindingType}");
                    }
                }
                else if (lambda.Body is NewExpression newExp)
                {
                    for (int i = 0; i < newExp.Members.Count; i++)
                    {
                        var member = newExp.Members[i];
                        var arg = newExp.Arguments[i];
                        if (arg.NodeType != ExpressionType.MemberAccess)
                            throw new NotImplementedException($"NodeType：{arg.NodeType}");

                        var argMember = (MemberExpression)arg;
                        if (argMember.Expression.NodeType != ExpressionType.Parameter)
                            throw new NotImplementedException($"BindingType：{argMember.Expression.NodeType}");

                        var table = DbObject.Get(argMember.Member.ReflectedType);
                        var alias = context.Alias[table.Type];
                        fields.Add($"{alias.Alias}.{_sqlSyntax.PropertyNameAlias(new DbAlias(table[argMember.Member.Name].DbName, member.Name))}");
                    }
                }
                else
                    throw new NotImplementedException($"NodeType：{lambda.Body.NodeType}");

                return fields;
            }

            if (context.JoinMetedatas.Count > 0)
                return new[] { $"{context.Alias[context.DbObject.Type].Alias}.*" };
            else
            {
                var alias = context.Alias[context.DbObject.Type];
                return context.DbObject.Properties.Select(o => $"{alias.Alias}.{o.EscapeNameAsAlias}");
            }
        }

        string GetOrderBy(QueryContext context)
        {
            if (context.OrderByMetedata == null)
                return null;

            var orderBy = GetOrderByFields(context, context.OrderByMetedata);
            if (context.ThenByMetedata == null)
                return _sqlSyntax.OrderBy(orderBy, context.OrderByMetedata.SortType, null, null);

            var thenBy = GetOrderByFields(context, context.ThenByMetedata);
            return _sqlSyntax.OrderBy(orderBy, context.OrderByMetedata.SortType, thenBy, context.ThenByMetedata.SortType);
        }

        IEnumerable<string> GetOrderByFields(QueryContext context, OrderByMetedata orderByMetedata)
        {
            foreach (var exp in orderByMetedata.Expressions)
            {
                yield return GetPropertyName(exp, context);
            }
        }

        string GetPropertyName(Expression exp, QueryContext context)
        {
            Expression expression = ((LambdaExpression)exp).Body;
            if (expression.NodeType == ExpressionType.Convert)
                expression = ((UnaryExpression)expression).Operand;

            if (expression.NodeType != ExpressionType.MemberAccess)
                throw new ArgumentException("请正确设置排序字段");

            var m = (MemberExpression)expression;
            if (m.Expression.NodeType != ExpressionType.Parameter)
                throw new ArgumentException("请正确设置排序字段");

            return string.Format("{0}.{1}", context.Alias[m.Member.ReflectedType].Alias, DbObject.Get(m.Member.ReflectedType)[m.Member.Name].EscapeName);
        }

        IEnumerable<string> GetJoins(QueryContext context, PredicateExpressionParser parser)
        {
            if (context.JoinMetedatas.Count > 0)
                return context.JoinMetedatas.Select(o => _sqlSyntax.Join(_sqlSyntax.TableNameAlias(context.Alias[o.DbObject.Type]), o.Type, parser.ToSql(o.JoinExpression, context)));
            return null;
        }
    }
}
