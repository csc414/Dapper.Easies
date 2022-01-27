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
                GetFields(context, aggregateInfo, parameterBuilder),
                GetJoins(context, parser),
                GetPredicate(context.WhereExpressions, context, parser),
                GetGroupBy(context, parameterBuilder),
                GetPredicate(context.HavingExpressions, context, parser),
                aggregateInfo == null ? GetOrderBy(context, parameterBuilder) : null,
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
                string.Join(_sqlSyntax.Operator(OperatorType.AndAlso), primaryKeys.Select((o, i) =>
                {
                    var name = _sqlSyntax.ParameterName(o.PropertyInfo.Name);
                    dynamicParameters.Add(name, ids[i]);
                    return $"{o.EscapeName} = {name}";
                })),
                null,
                null,
                null,
                0,
                1);
            parameters = dynamicParameters;
            if (_options.DevelopmentMode)
                _logger?.LogParametersSql(sql, parameters);
            return sql;
        }

        public string ToInsertSql<T>(out bool hasIdentityKey)
        {
            var table = DbObject.Get(typeof(T));
            hasIdentityKey = table.IdentityKey != null;
            var properties = table.Properties.Where(o => !o.IdentityKey);
            var sql = _sqlSyntax.InsertFormat(
                table.EscapeName,
                properties.Select(o => o.EscapeName),
                properties.Select(o => _sqlSyntax.ParameterName(o.PropertyInfo.Name)),
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

            var sql = _sqlSyntax.DeleteFormat(
                table.EscapeName,
                null,
                null,
                string.Join(_sqlSyntax.Operator(OperatorType.AndAlso), primaryKeys.Select(o => $"{o.EscapeName} = {_sqlSyntax.ParameterName(o.PropertyInfo.Name)}")));
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
                GetPredicate(context.WhereExpressions, context, parser));
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
                    var table = DbObject.Get(binding.Member.ReflectedType);
                    var alias = context.Alias[table.Type];
                    var value = ExpressionParser.GetExpression(assignment.Expression, parameterBuilder, _sqlSyntax, context);
                    fields.Add($"{alias.Alias}.{table[binding.Member.Name].EscapeName} = {value}");
                }
                else
                    throw new NotImplementedException($"BindingType：{binding.BindingType}");
            }

            var sql = _sqlSyntax.UpdateFormat(
                _sqlSyntax.TableNameAlias(context.Alias.Values.First()),
                fields,
                GetPredicate(context.WhereExpressions, context, parser));
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

            var properties = table.Properties.Where(o => !o.PrimaryKey);
            var sql = _sqlSyntax.UpdateFormat(
                table.EscapeName,
                properties.Select(o => $"{o.EscapeName} = {_sqlSyntax.ParameterName(o.PropertyInfo.Name)}"),
                string.Join(_sqlSyntax.Operator(OperatorType.AndAlso), primaryKeys.Select(o => $"{o.EscapeName} = {_sqlSyntax.ParameterName(o.PropertyInfo.Name)}")));
            if (_options.DevelopmentMode)
                _logger?.LogSql(sql);
            return sql;
        }

        string GetPredicate(IEnumerable<Expression> expressions, QueryContext context, PredicateExpressionParser parser)
        {
            if (expressions?.Any() == true)
                return string.Join(_sqlSyntax.Operator(OperatorType.AndAlso), expressions.Select(o => parser.ToSql(o, context)));
            return null;
        }

        IEnumerable<string> GetFields(QueryContext context, AggregateInfo aggregateInfo, ParameterBuilder builder)
        {
            if (aggregateInfo != null)
            {
                var expr = ExpressionParser.GetExpression(aggregateInfo.Expression, builder, _sqlSyntax, context);
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
                var fields = new List<string>();
                var lambda = (LambdaExpression)context.SelectorExpression;
                if (lambda.Body is MemberInitExpression initExp)
                {
                    foreach (var binding in initExp.Bindings)
                    {
                        if (binding is MemberAssignment assignment)
                        {
                            fields.Add($"{_sqlSyntax.PropertyNameAlias(new DbAlias(ExpressionParser.GetExpression(assignment.Expression, builder, _sqlSyntax, context), assignment.Member.Name, true))}");
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
                        fields.Add($"{_sqlSyntax.PropertyNameAlias(new DbAlias(ExpressionParser.GetExpression(arg, builder, _sqlSyntax, context), member.Name, true))}");
                    }
                }
                else if (lambda.Body is MemberExpression memberExp)
                    return new[] { ExpressionParser.GetExpression(memberExp, builder, _sqlSyntax, context) };
                else
                    throw new NotImplementedException($"NodeType：{lambda.Body.NodeType}");

                return fields;
            }

            if (context.JoinMetedatas?.Any() == true)
                return new[] { $"{context.Alias[context.DbObject.Type].Alias}.*" };
            else
            {
                var alias = context.Alias[context.DbObject.Type];
                return context.DbObject.Properties.Select(o => $"{alias.Alias}.{o.EscapeNameAsAlias}");
            }
        }

        string GetOrderBy(QueryContext context, ParameterBuilder builder)
        {
            if (context.OrderByMetedata == null)
                return null;

            var orderBy = GetOrderByFields(context, builder, context.OrderByMetedata);
            if (context.ThenByMetedata == null)
                return _sqlSyntax.OrderBy(orderBy, context.OrderByMetedata.SortType, null, null);

            var thenBy = GetOrderByFields(context, builder, context.ThenByMetedata);
            return _sqlSyntax.OrderBy(orderBy, context.OrderByMetedata.SortType, thenBy, context.ThenByMetedata.SortType);
        }

        string GetGroupBy(QueryContext context, ParameterBuilder builder)
        {
            if (context.GroupByExpression == null)
                return null;

            var lambda = (LambdaExpression)context.GroupByExpression;
            if (lambda.Body is MemberExpression memberExp)
                return _sqlSyntax.GroupBy(new[] { ExpressionParser.GetExpression(memberExp, builder, _sqlSyntax, context) });
            else if (lambda.Body is NewExpression newExp)
            {
                var fields = new List<string>();
                for (int i = 0; i < newExp.Members.Count; i++)
                {
                    var arg = newExp.Arguments[i];
                    fields.Add(ExpressionParser.GetExpression(arg, builder, _sqlSyntax, context));
                }

                return _sqlSyntax.GroupBy(fields);
            }

            throw new NotImplementedException($"NodeType：{lambda.Body.NodeType}");
        }

        IEnumerable<string> GetOrderByFields(QueryContext context, ParameterBuilder builder, OrderByMetedata orderByMetedata)
        {
            foreach (var exp in orderByMetedata.Expressions)
            {
                yield return ExpressionParser.GetExpression(exp, builder, _sqlSyntax, context);
            }
        }

        IEnumerable<string> GetJoins(QueryContext context, PredicateExpressionParser parser)
        {
            if (context.JoinMetedatas?.Any() == true)
                return context.JoinMetedatas.Select(o => _sqlSyntax.Join(_sqlSyntax.TableNameAlias(context.Alias[o.DbObject.Type]), o.Type, parser.ToSql(o.JoinExpression, context)));
            return null;
        }
    }
}
