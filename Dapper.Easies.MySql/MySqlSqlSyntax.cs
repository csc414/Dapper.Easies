using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies.MySql
{
    public class MySqlSqlSyntax : DefaultSqlSyntax
    {
        public override string SelectFormat(string tableName, IEnumerable<string> fields, IEnumerable<string> joins, string where, string orderBy, int skip, int take)
        {
            var sql = base.SelectFormat(tableName, fields, joins, where, orderBy, skip, take);
            if (take > 0)
                sql = $"{sql} LIMIT {skip},{take}";

            return sql;
        }

        public override string InsertFormat(string tableName, IEnumerable<string> fields, IEnumerable<string> paramNames, bool hasIdentityKey)
        {
            var sql = base.InsertFormat(tableName, fields, paramNames, hasIdentityKey);
            if(hasIdentityKey)
                return $"{sql}; SELECT LAST_INSERT_ID()";

            return sql;
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
