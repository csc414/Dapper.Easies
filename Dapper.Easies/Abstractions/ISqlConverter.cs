using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
    public interface ISqlConverter
    {
        string ToQuerySql(QueryContext context, out DynamicParameters parameters, int? take = null, AggregateInfo aggregateInfo = null);

        string ToInsertSql<T>(T entity, out DynamicParameters parameters, out bool hasIdentityKey);

        string ToDeleteSql(QueryContext context, bool correlation, out DynamicParameters parameters);

        string ToUpdateSql<T>(T entity, out DynamicParameters parameters);

        string ToUpdateFieldsSql(Expression updateFieldsExp, QueryContext context, out DynamicParameters parameters);
    }
}
