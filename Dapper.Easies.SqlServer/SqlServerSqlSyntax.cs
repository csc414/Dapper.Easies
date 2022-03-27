using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Dapper.Easies.SqlServer
{
    public class SqlServerSqlSyntax : DefaultSqlSyntax
    {
        internal static SqlServerSqlSyntax Instance { get; } = new SqlServerSqlSyntax();

        public override string SelectFormat(string tableName, IEnumerable<string> fields, IEnumerable<string> joins, string where, string groupBy, string having, string orderBy, int skip, int take, bool distinct)
        {
            var sql = new StringBuilder("SELECT");
            if (distinct)
                sql.AppendFormat(" DISTINCT");

            if (skip == 0 && take > 0)
                sql.AppendFormat(" TOP {0}", take);

            sql.AppendFormat(" {0}", string.Join(", ", fields));
            sql.AppendFormat(" FROM {0}", tableName);

            if (joins != null)
                sql.AppendFormat(" {0}", string.Join(" ", joins));

            if (where != null)
                sql.AppendFormat(" WHERE {0}", where);

            if (groupBy != null)
                sql.AppendFormat(" {0}", groupBy);

            if (having != null)
                sql.AppendFormat(" HAVING {0}", having);

            if (orderBy != null)
                sql.AppendFormat(" {0}", orderBy);

            if (skip > 0 && take > 0)
                sql.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", skip, take);

            return sql.ToString();
        }

        public override string InsertFormat(string tableName, IEnumerable<string> fields, IEnumerable<string> paramNames, bool hasIdentityKey)
        {
            var sql = base.InsertFormat(tableName, fields, paramNames, hasIdentityKey);
            if (hasIdentityKey)
                return $"{sql}; SELECT @@IDENTITY";

            return sql;
        }

        public override string EscapePropertyName(string name)
        {
            return $"[{name}]";
        }

        public override string EscapeTableName(string name)
        {
            return $"[{name}]";
        }

        public override string UpdateFormat(string tableName, string tableAlias, IEnumerable<string> updateFields, string where)
        {
            var sql = new StringBuilder($"UPDATE {tableAlias ?? tableName} SET {string.Join(", ", updateFields)}");
            if (tableAlias != null)
                sql.AppendFormat(" FROM {0} {1}", tableName, tableAlias);

            if (where != null)
                sql.AppendFormat(" WHERE {0}", where);

            return sql.ToString();
        }

        public override string DateTimeMethod(string name, Func<string> getPropertyName)
        {
            switch (name)
            {
                case "Date":
                    return $"CONVERT(varchar(10), {getPropertyName()}, 23)";
                case "Hour":
                    return $"DATEPART(HOUR, {getPropertyName()})";
                case "Minute":
                    return $"DATEPART(MINUTE, {getPropertyName()})";
                case "Second":
                    return $"DATEPART(SECOND, {getPropertyName()})";
                default:
                    return base.DateTimeMethod(name, getPropertyName);
            }
        }
    }
}
