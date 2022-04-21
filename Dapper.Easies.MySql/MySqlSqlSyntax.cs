using System;
using System.Collections.Generic;

namespace Dapper.Easies.MySql
{
    public class MySqlSqlSyntax : DefaultSqlSyntax
    {
        internal static MySqlSqlSyntax Instance { get; } = new MySqlSqlSyntax();

        public override string SelectFormat(string tableName, IEnumerable<string> fields, IEnumerable<string> joins, string where, string groupBy, string having, string orderBy, int skip, int take, bool distinct)
        {
            var sql = base.SelectFormat(tableName, fields, joins, where, groupBy, having, orderBy, skip, take, distinct);
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

        public override string DateTimeMethod(string name, Func<string> getPropertyName)
        {
            switch (name)
            {
                case "Date":
                    return $"DATE({getPropertyName()})";
                case "Hour":
                    return $"HOUR({getPropertyName()})";
                case "Minute":
                    return $"MINUTE({getPropertyName()})";
                case "Second":
                    return $"SECOND({getPropertyName()})";
                default:
                    return base.DateTimeMethod(name, getPropertyName);
            }
        }
    }
}
