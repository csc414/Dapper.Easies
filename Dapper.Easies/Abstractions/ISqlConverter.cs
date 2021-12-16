using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies
{
    public interface ISqlConverter
    {
        string ToQuerySql(QueryContext context, out DynamicParameters parameters);

        string ToInsertSql<T>(T entity, out DynamicParameters parameters, out bool hasIdentityKey);

        string ToDeleteSql(QueryContext context, bool correlation, out DynamicParameters parameters);
    }
}
