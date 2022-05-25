using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Dapper.Easies
{
    internal class DefaultSqlConverter : ISqlConverter
    {
        private readonly EasiesOptions _options;

        private readonly ILogger _logger;

        public DefaultSqlConverter(EasiesOptions options, ILoggerFactory factory = null)
        {
            _options = options;
            _logger = factory?.CreateLogger<DefaultSqlConverter>();
            DbObject.Initialize(options);
        }

        public string ToQuerySql(QueryContext context, ParameterBuilder parameterBuilder)
        {
            var sqlSyntax = context.DbObject.SqlSyntax;
            var parser = new PredicateExpressionParser(sqlSyntax, parameterBuilder);
            var sql = sqlSyntax.SelectFormat(
                sqlSyntax.TableNameAlias(context.Alias[0]),
                GetFields(sqlSyntax, context, null, parameterBuilder),
                GetJoins(sqlSyntax, context, parser),
                GetPredicate(sqlSyntax, context.WhereExpressions, context, parser),
                GetGroupBy(sqlSyntax, context, parameterBuilder),
                GetPredicate(sqlSyntax, context.HavingExpressions, context, parser),
                GetOrderBy(sqlSyntax, context, parameterBuilder),
                context.Skip,
                context.Take,
                context.Distinct);
            return sql;
        }

        public string ToQuerySql(QueryContext context, out DynamicParameters parameters, int? skip = null, int? take = null, AggregateInfo aggregateInfo = null)
        {
            var sqlSyntax = context.DbObject.SqlSyntax;
            var parameterBuilder = new ParameterBuilder(sqlSyntax);
            var parser = new PredicateExpressionParser(sqlSyntax, parameterBuilder);
            var sql = sqlSyntax.SelectFormat(
                sqlSyntax.TableNameAlias(context.Alias[0]),
                GetFields(sqlSyntax, context, aggregateInfo, parameterBuilder),
                GetJoins(sqlSyntax, context, parser),
                GetPredicate(sqlSyntax, context.WhereExpressions, context, parser),
                GetGroupBy(sqlSyntax, context, parameterBuilder),
                GetPredicate(sqlSyntax, context.HavingExpressions, context, parser),
                aggregateInfo == null ? GetOrderBy(sqlSyntax, context, parameterBuilder) : null,
                skip ?? context.Skip,
                take ?? context.Take,
                context.Distinct);
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

            var sqlSyntax = table.SqlSyntax;
            var dynamicParameters = new DynamicParameters();
            var sql = sqlSyntax.SelectFormat(
                table.EscapeName,
                table.Properties.Select(o => o.EscapeNameAsAlias),
                null,
                string.Join(sqlSyntax.Operator(OperatorType.AndAlso), primaryKeys.Select((o, i) =>
                {
                    var name = sqlSyntax.ParameterName(o.PropertyInfo.Name);
                    dynamicParameters.Add(name, ids[i]);
                    return $"{o.EscapeName} = {name}";
                })),
                null,
                null,
                null,
                0,
                1,
                false);
            parameters = dynamicParameters;
            if (_options.DevelopmentMode)
                _logger?.LogParametersSql(sql, parameters);
            return sql;
        }

        public string ToInsertSql<T>(out bool hasIdentityKey)
        {
            var table = DbObject.Get(typeof(T));
            var sqlSyntax = table.SqlSyntax;
            hasIdentityKey = table.IdentityKey != null;
            var properties = table.Properties.Where(o => !o.IdentityKey);
            var sql = sqlSyntax.InsertFormat(
                table.EscapeName,
                properties.Select(o => o.EscapeName),
                properties.Select(o => sqlSyntax.ParameterName(o.PropertyInfo.Name)),
                hasIdentityKey);
            if (_options.DevelopmentMode)
                _logger?.LogSql(sql);
            return sql;
        }

        public string ToDeleteSql<T>()
        {
            var table = DbObject.Get(typeof(T));
            var primaryKeys = table.Properties.Where(o => o.PrimaryKey).ToArray();
            if (primaryKeys.Length == 0)
                throw new ArgumentException("删除失败，实体类没有主键");

            var sqlSyntax = table.SqlSyntax;
            var sql = sqlSyntax.DeleteFormat(
                table.EscapeName,
                null,
                null,
                string.Join(sqlSyntax.Operator(OperatorType.AndAlso), primaryKeys.Select(o => $"{o.EscapeName} = {sqlSyntax.ParameterName(o.PropertyInfo.Name)}")));
            if (_options.DevelopmentMode)
                _logger?.LogSql(sql);
            return sql;
        }

        public string ToDeleteSql(QueryContext context, out DynamicParameters parameters)
        {
            var sqlSyntax = context.DbObject.SqlSyntax;
            var parameterBuilder = new ParameterBuilder(sqlSyntax);
            var parser = new PredicateExpressionParser(sqlSyntax, parameterBuilder);
            var tableAlias = context.Alias[0];
            var sql = sqlSyntax.DeleteFormat(
                tableAlias.Name,
                tableAlias.Alias,
                GetJoins(sqlSyntax, context, parser),
                GetPredicate(sqlSyntax, context.WhereExpressions, context, parser));
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

            var sqlSyntax = context.DbObject.SqlSyntax;
            var parameterBuilder = new ParameterBuilder(sqlSyntax);
            var parser = new PredicateExpressionParser(sqlSyntax, parameterBuilder);
            var fields = new List<string>();
            var alias = context.Alias[0];
            foreach (var binding in initExp.Bindings)
            {
                if (binding is MemberAssignment assignment)
                {
                    var table = DbObject.Get(binding.Member.ReflectedType);
                    var value = ExpressionParser.GetExpression(assignment.Expression, parameterBuilder, sqlSyntax, context, lambda?.Parameters);
                    fields.Add($"{alias.Alias}.{table[binding.Member.Name].EscapeName} = {value}");
                }
                else
                    throw new NotImplementedException($"BindingType：{binding.BindingType}");
            }

            var sql = sqlSyntax.UpdateFormat(
                alias.Name,
                alias.Alias,
                fields,
                GetPredicate(sqlSyntax, context.WhereExpressions, context, parser));
            parameters = parameterBuilder.GetDynamicParameters();
            if (_options.DevelopmentMode)
                _logger?.LogParametersSql(sql, parameters);
            return sql;
        }

        public string ToUpdateSql<T>()
        {
            var table = DbObject.Get(typeof(T));
            var primaryKeys = table.Properties.Where(o => o.PrimaryKey).ToArray();
            if (primaryKeys.Length == 0)
                throw new ArgumentException("更新失败，实体类没有主键");

            var sqlSyntax = table.SqlSyntax;
            var properties = table.Properties.Where(o => !o.PrimaryKey);
            var sql = sqlSyntax.UpdateFormat(
                table.EscapeName,
                null,
                properties.Select(o => $"{o.EscapeName} = {sqlSyntax.ParameterName(o.PropertyInfo.Name)}"),
                string.Join(sqlSyntax.Operator(OperatorType.AndAlso), primaryKeys.Select(o => $"{o.EscapeName} = {sqlSyntax.ParameterName(o.PropertyInfo.Name)}")));
            if (_options.DevelopmentMode)
                _logger?.LogSql(sql);
            return sql;
        }

        string GetPredicate(ISqlSyntax sqlSyntax, IEnumerable<Expression> expressions, QueryContext context, PredicateExpressionParser parser)
        {
            var count = expressions?.Count();
            if (count > 0)
            {
                var sqls = expressions.Select(o =>
                {
                    var sql = parser.ToSql(o, context);
                    if (count > 1)
                        return $"({sql})";
                    return sql;
                });
                return string.Join(sqlSyntax.Operator(OperatorType.AndAlso), sqls);
            }
            return null;
        }

        IEnumerable<string> GetFields(ISqlSyntax sqlSyntax, QueryContext context, AggregateInfo aggregateInfo, ParameterBuilder builder)
        {
            int aliasIndex = 0;
            DbObject specificDbObject = null;
            if (aggregateInfo != null)
            {
                var lambda = (LambdaExpression)aggregateInfo.Expression;
                var args = lambda == null ? Array.Empty<Expression>() : new[] { lambda.Body };
                var result = sqlSyntax.Method(aggregateInfo.Type.ToString(), args, builder, GetExpr, GetValue);
                return new[] { result };

                string GetExpr(Expression exp)
                {
                    if (exp == null)
                        return null;

                    return ExpressionParser.GetExpression(exp, builder, sqlSyntax, context, lambda?.Parameters);
                }

                object GetValue(Expression exp)
                {
                    if (exp == null)
                        return null;

                    return ExpressionParser.GetValue(exp);
                }
            }

            if (context.SelectorExpression != null)
            {
                var lambda = (LambdaExpression)context.SelectorExpression;
                if (lambda.Body is MemberInitExpression initExp)
                {
                    var fields = new List<string>();
                    foreach (var binding in initExp.Bindings)
                    {
                        if (binding is MemberAssignment assignment)
                        {
                            fields.Add(sqlSyntax.PropertyNameAlias(new DbAlias(ExpressionParser.GetExpression(assignment.Expression, builder, sqlSyntax, context, lambda?.Parameters), assignment.Member.Name, true)));
                        }
                        else
                            throw new NotImplementedException($"BindingType：{binding.BindingType}");
                    }
                    return fields;
                }
                else if (lambda.Body is NewExpression newExp)
                {
                    var fields = new List<string>();
                    for (int i = 0; i < newExp.Members.Count; i++)
                    {
                        var member = newExp.Members[i];
                        var arg = newExp.Arguments[i];
                        fields.Add(sqlSyntax.PropertyNameAlias(new DbAlias(ExpressionParser.GetExpression(arg, builder, sqlSyntax, context, lambda?.Parameters), member.Name, true)));
                    }
                    return fields;
                }
                else if (lambda.Body.NodeType == ExpressionType.MemberAccess || lambda.Body.NodeType == ExpressionType.Call)
                    return new[] { ExpressionParser.GetExpression(lambda.Body, builder, sqlSyntax, context, lambda?.Parameters) };
                else if (lambda.Body is ParameterExpression parameter)
                {
                    aliasIndex = lambda.Parameters.IndexOf(parameter);
                    specificDbObject = DbObject.Get(parameter.Type);
                    if (specificDbObject == null)
                    {
                        var tableAlias = context.Alias[aliasIndex];
                        return parameter.Type.GetProperties().Select(o => $"{tableAlias.Alias}.{sqlSyntax.EscapePropertyName(o.Name)}");
                    }
                }
                else
                    throw new NotImplementedException($"NodeType：{lambda.Body.NodeType}");
            }

            var alias = context.Alias[aliasIndex];
            return (specificDbObject ?? context.DbObject).Properties.Select(o => $"{alias.Alias}.{o.EscapeNameAsAlias}");
        }

        string GetOrderBy(ISqlSyntax sqlSyntax, QueryContext context, ParameterBuilder builder)
        {
            if (context.OrderByMetedata == null)
                return null;

            var orderBy = GetOrderByFields(sqlSyntax, context, builder, context.OrderByMetedata);
            if (context.ThenByMetedata == null)
                return sqlSyntax.OrderBy(orderBy, context.OrderByMetedata.SortType, null, null);

            var thenBy = GetOrderByFields(sqlSyntax, context, builder, context.ThenByMetedata);
            return sqlSyntax.OrderBy(orderBy, context.OrderByMetedata.SortType, thenBy, context.ThenByMetedata.SortType);
        }

        string GetGroupBy(ISqlSyntax sqlSyntax, QueryContext context, ParameterBuilder builder)
        {
            if (context.GroupByExpression == null)
                return null;

            var lambda = (LambdaExpression)context.GroupByExpression;
            if (lambda.Body is MemberExpression memberExp)
                return sqlSyntax.GroupBy(new[] { ExpressionParser.GetExpression(memberExp, builder, sqlSyntax, context, lambda.Parameters) });
            else if (lambda.Body is NewExpression newExp)
            {
                var fields = new List<string>();
                for (int i = 0; i < newExp.Members.Count; i++)
                {
                    var arg = newExp.Arguments[i];
                    fields.Add(ExpressionParser.GetExpression(arg, builder, sqlSyntax, context, lambda?.Parameters));
                }

                return sqlSyntax.GroupBy(fields);
            }

            throw new NotImplementedException($"NodeType：{lambda.Body.NodeType}");
        }

        IEnumerable<string> GetOrderByFields(ISqlSyntax sqlSyntax, QueryContext context, ParameterBuilder builder, OrderByMetedata orderByMetedata)
        {
            foreach (var exp in orderByMetedata.Expressions)
            {
                var lambda = (LambdaExpression)exp;
                yield return ExpressionParser.GetExpression(lambda, builder, sqlSyntax, context, lambda?.Parameters);
            }
        }

        IEnumerable<string> GetJoins(ISqlSyntax sqlSyntax, QueryContext context, PredicateExpressionParser parser)
        {
            if (context.JoinMetedatas?.Any() == true)
                return context.JoinMetedatas.Select((o, i) =>
                {
                    var alias = context.Alias[i + 1];
                    string tableName;
                    if (o.DbObject == null)
                    {
                        tableName = sqlSyntax.TableNameAlias(new DbAlias($"({o.Query.Context.Converter.ToQuerySql(o.Query.Context, parser.ParameterBuilder)})", alias.Alias));
                    }
                    else
                        tableName = sqlSyntax.TableNameAlias(alias);

                    return sqlSyntax.Join(tableName, o.Type, parser.ToSql(o.JoinExpression, context));
                });
            return null;
        }
    }
}
