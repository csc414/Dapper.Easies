using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies
{
    public static class DbQueryExtensions
    {
        public static (string sql, DynamicParameters parameters) GetSql<T>(this T query, int? take = null, AggregateInfo aggregateInfo = null) where T : IDbQuery
        {
            var sql = query.Context.Converter.ToQuerySql(query.Context, out var parameters, take, aggregateInfo);
            return (sql, parameters);
        }
    }
}
