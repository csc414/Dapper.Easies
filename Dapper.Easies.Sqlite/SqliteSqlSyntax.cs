﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Dapper.Easies.MySql
{
    public class SqliteSqlSyntax : DefaultSqlSyntax
    {
        internal static SqliteSqlSyntax Instance { get; } = new SqliteSqlSyntax();

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
                return $"{sql}; SELECT MAX(LAST_INSERT_ROWID()) FROM {tableName}";

            return sql;
        }

        public override string UpdateFormat(string tableName, string tableAlias, IEnumerable<string> updateFields, string where)
        {
            var sql = new StringBuilder($"UPDATE {tableName} SET {string.Join(", ", updateFields)}");

            if (where != null)
                sql.AppendFormat(" WHERE {0}", where);

            if (tableAlias != null)
                sql.Replace($"{tableAlias}.", "");

            return sql.ToString();
        }

        public override string DeleteFormat(string tableName, string tableAlias, IEnumerable<string> joins, string where)
        {
            var sql = new StringBuilder($"DELETE FROM {tableName}");

            if (where != null)
                sql.AppendFormat(" WHERE {0}", where);

            if (tableAlias != null)
                sql.Replace($"{tableAlias}.", "");

            return sql.ToString();
        }

        public override string EscapePropertyName(string name)
        {
            return $"[{name}]";
        }

        public override string EscapeTableName(string name)
        {
            return $"[{name}]";
        }

        public override string DateTimeMethod(string name, Func<string> getPropertyName)
        {
            throw new NotImplementedException();
        }
    }
}
