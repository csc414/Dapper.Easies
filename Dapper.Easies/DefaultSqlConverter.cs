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

        private readonly ILogger _logger;

        public DefaultSqlConverter(EasiesOptions options, ILoggerFactory factory = null)
        {
            _options = options;
            _logger = factory?.CreateLogger<DefaultSqlConverter>();
            DbObject.Initialize(options);
        }

        public string ToQuerySql(QueryContext context, out DynamicParameters parameters, int? take = null, AggregateInfo aggregateInfo = null)
        {
            var sqlSyntax = context.DbObject.SqlSyntax;
            var parameterBuilder = new ParameterBuilder(sqlSyntax);
            var parser = new PredicateExpressionParser(sqlSyntax, parameterBuilder);
            var sql = sqlSyntax.SelectFormat(
                sqlSyntax.TableNameAlias(context.Alias.Values.First()),
                GetFields(sqlSyntax, context, aggregateInfo, parameterBuilder),
                GetJoins(sqlSyntax, context, parser),
                GetPredicate(sqlSyntax, context.WhereExpressions, context, parser),
                GetGroupBy(sqlSyntax, context, parameterBuilder),
                GetPredicate(sqlSyntax, context.HavingExpressions, context, parser),
                aggregateInfo == null ? GetOrderBy(sqlSyntax, context, parameterBuilder) : null,
                context.Skip,
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
            var tableAlias = context.Alias.Values.First();
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
            foreach (var binding in initExp.Bindings)
            {
                if (binding is MemberAssignment assignment)
                {
                    var table = DbObject.Get(binding.Member.ReflectedType);
                    var alias = context.Alias[table.Type];
                    var value = ExpressionParser.GetExpression(assignment.Expression, parameterBuilder, sqlSyntax, context);
                    fields.Add($"{alias.Alias}.{table[binding.Member.Name].EscapeName} = {value}");
                }
                else
                    throw new NotImplementedException($"BindingType：{binding.BindingType}");
            }

            var tableAlias = context.Alias.Values.First();
            var sql = sqlSyntax.UpdateFormat(
                tableAlias.Name,
                tableAlias.Alias,
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
            if (expressions?.Any() == true)
                return string.Join(sqlSyntax.Operator(OperatorType.AndAlso), expressions.Select(o => parser.ToSql(o, context)));
            return null;
        }

        IEnumerable<string> GetFields(ISqlSyntax sqlSyntax, QueryContext context, AggregateInfo aggregateInfo, ParameterBuilder builder)
        {
            DbObject specificDbObject = null;
            if (aggregateInfo != null)
            {
                var expr = ExpressionParser.GetExpression(aggregateInfo.Expression, builder, sqlSyntax, context);
                switch (aggregateInfo.Type)
                {
                    case AggregateType.Count:
                        var field = "*";
                        if (aggregateInfo.Expression != null)
                            field = expr;
                        return new[] { $"COUNT({field})" };
                    case AggregateType.Max:
                        return new[] { $"MAX({expr})" };
                    case AggregateType.Min:
                        return new[] { $"MIN({expr})" };
                    case AggregateType.Avg:
                        return new[] { $"AVG({expr})" };
                    case AggregateType.Sum:
                        return new[] { $"SUM({expr})" };
                    default:
                        throw new NotImplementedException($"AggregateType：{aggregateInfo.Type}");
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
                            fields.Add($"{sqlSyntax.PropertyNameAlias(new DbAlias(ExpressionParser.GetExpression(assignment.Expression, builder, sqlSyntax, context), assignment.Member.Name, true))}");
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
                        fields.Add($"{sqlSyntax.PropertyNameAlias(new DbAlias(ExpressionParser.GetExpression(arg, builder, sqlSyntax, context), member.Name, true))}");
                    }
                    return fields;
                }
                else if (lambda.Body.NodeType == ExpressionType.MemberAccess || lambda.Body.NodeType == ExpressionType.Call)
                    return new[] { ExpressionParser.GetExpression(lambda.Body, builder, sqlSyntax, context) };
                else if (lambda.Body is ParameterExpression parameter)
                    specificDbObject = DbObject.Get(parameter.Type);
                else
                    throw new NotImplementedException($"NodeType：{lambda.Body.NodeType}");
            }

            var alias = context.Alias[specificDbObject?.Type ?? context.DbObject.Type];
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
                return sqlSyntax.GroupBy(new[] { ExpressionParser.GetExpression(memberExp, builder, sqlSyntax, context) });
            else if (lambda.Body is NewExpression newExp)
            {
                var fields = new List<string>();
                for (int i = 0; i < newExp.Members.Count; i++)
                {
                    var arg = newExp.Arguments[i];
                    fields.Add(ExpressionParser.GetExpression(arg, builder, sqlSyntax, context));
                }

                return sqlSyntax.GroupBy(fields);
            }

            throw new NotImplementedException($"NodeType：{lambda.Body.NodeType}");
        }

        IEnumerable<string> GetOrderByFields(ISqlSyntax sqlSyntax, QueryContext context, ParameterBuilder builder, OrderByMetedata orderByMetedata)
        {
            foreach (var exp in orderByMetedata.Expressions)
            {
                yield return ExpressionParser.GetExpression(exp, builder, sqlSyntax, context);
            }
        }

        IEnumerable<string> GetJoins(ISqlSyntax sqlSyntax, QueryContext context, PredicateExpressionParser parser)
        {
            if (context.JoinMetedatas?.Any() == true)
                return context.JoinMetedatas.Select(o => sqlSyntax.Join(sqlSyntax.TableNameAlias(context.Alias[o.DbObject.Type]), o.Type, parser.ToSql(o.JoinExpression, context)));
            return null;
        }
    }
}
