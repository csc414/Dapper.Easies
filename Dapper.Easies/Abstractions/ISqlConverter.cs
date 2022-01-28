using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
    public interface ISqlConverter
    {
        string ToQuerySql(QueryContext context, out DynamicParameters parameters, int? take = null, AggregateInfo aggregateInfo = null);

        string ToGetSql<T>(object[] ids, out DynamicParameters parameters);

        string ToInsertSql<T>(out bool hasIdentityKey);

        string ToDeleteSql<T>();

        string ToDeleteSql(QueryContext context, out DynamicParameters parameters);

        string ToUpdateSql<T>();

        string ToUpdateFieldsSql(Expression updateFieldsExp, QueryContext context, out DynamicParameters parameters);
    }
}
