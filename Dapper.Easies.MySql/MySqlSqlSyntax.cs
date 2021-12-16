using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies.MySql
{
    public class MySqlSqlSyntax : DefaultSqlSyntax
    {
        public override string QueryFormat(string tableName, string fields, IEnumerable<string> joins, string where, string orderBy, int skip, int take)
        {
            var sql = base.QueryFormat(tableName, fields, joins, where, orderBy, skip, take);
            if (take > 0)
                sql = $"{sql} limit {skip},{take}";

            return sql;
        }

        public override string InsertFormat(string tableName, IEnumerable<string> fields, IEnumerable<string> paramNames, bool hasIdentityKey)
        {
            return $"{base.InsertFormat(tableName, fields, paramNames, hasIdentityKey)}; select LAST_INSERT_ID()";
        }

        public override string EscapePropertyName(string name)
        {
            return $"`{name}`";
        }

        public override string EscapeTableName(string name)
        {
            return $"`{name}`";
        }
    }
}
