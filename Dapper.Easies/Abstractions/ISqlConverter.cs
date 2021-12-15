using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies
{
    public interface ISqlConverter
    {
        string ToQuerySql(QueryContext context, out DynamicParameters parameters);
    }
}
