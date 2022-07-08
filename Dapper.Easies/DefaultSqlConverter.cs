using Microsoft.Extensions.Logging;
using System;
using System.Linq.Expressions;

namespace Dapper.Easies
{
    internal class DefaultSqlConverter : ISqlConverter
    {
        private readonly EasiesOptions _options;

        private readonly ILogger _logger;

        public DefaultSqlConverter(IServiceProvider serviceProvider, EasiesOptions options, ILoggerFactory factory = null)
        {
            _options = options;
            _logger = factory?.CreateLogger<DefaultSqlConverter>();
            DbObject.Initialize(serviceProvider, options);
        }

        public string ToQuerySql(QueryContext context, ParameterBuilder parameterBuilder)
        {
            return context.DbObject.SqlSyntax.SelectFormat(context, parameterBuilder);
        }

        public string ToQuerySql(QueryContext context, out DynamicParameters parameters, int? skip = null, int? take = null, AggregateInfo aggregateInfo = null)
        {
            var parameterBuilder = new ParameterBuilder();
            var sql = context.DbObject.SqlSyntax.SelectFormat(context, parameterBuilder, skip, take, aggregateInfo);
            parameters = parameterBuilder.GetDynamicParameters();
            if (_options.DevelopmentMode)
                _logger?.LogParametersSql(sql, parameters);
            return sql;
        }

        public string ToGetSql<T>(object[] ids, out DynamicParameters parameters)
        {
            var table = DbObject.Get(typeof(T));
            if(ids == null)
                throw new ArgumentException("参数异常");

            var parameterBuilder = new ParameterBuilder();
            var sql = table.SqlSyntax.SelectFormat(table, ids, parameterBuilder);
            parameters = parameterBuilder.GetDynamicParameters();
            if (_options.DevelopmentMode)
                _logger?.LogParametersSql(sql, parameters);
            return sql;
        }

        public string ToInsertSql<T>(out bool hasIdentityKey)
        {
            var table = DbObject.Get(typeof(T));
            hasIdentityKey = table.IdentityKey != null;
            var sql = table.SqlSyntax.InsertFormat(table, hasIdentityKey);
            if (_options.DevelopmentMode)
                _logger?.LogSql(sql);
            return sql;
        }

        public string ToDeleteSql<T>()
        {
            var table = DbObject.Get(typeof(T));
            var sql = table.SqlSyntax.DeleteFormat(table);
            if (_options.DevelopmentMode)
                _logger?.LogSql(sql);
            return sql;
        }

        public string ToDeleteSql(QueryContext context, out DynamicParameters parameters)
        {
            var parameterBuilder = new ParameterBuilder();
            var sql = context.DbObject.SqlSyntax.DeleteFormat(context, parameterBuilder);
            parameters = parameterBuilder.GetDynamicParameters();
            if (_options.DevelopmentMode)
                _logger?.LogParametersSql(sql, parameters);
            return sql;
        }

        public string ToUpdateFieldsSql(Expression updateFieldsExp, QueryContext context, out DynamicParameters parameters)
        {
            var lambda = (LambdaExpression)updateFieldsExp;
            if (lambda.Body is not MemberInitExpression)
                throw new NotImplementedException($"NodeType：{lambda.Body.NodeType}");

            var parameterBuilder = new ParameterBuilder();
            var sql = context.DbObject.SqlSyntax.UpdateFormat(lambda, context, parameterBuilder);
            parameters = parameterBuilder.GetDynamicParameters();
            if (_options.DevelopmentMode)
                _logger?.LogParametersSql(sql, parameters);
            return sql;
        }

        public string ToUpdateSql<T>()
        {
            var table = DbObject.Get(typeof(T));
            var sql = table.SqlSyntax.UpdateFormat(table);
            if (_options.DevelopmentMode)
                _logger?.LogSql(sql);
            return sql;
        }
    }
}
