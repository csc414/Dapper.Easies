using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
    public class DefaultSqlSyntax : ISqlSyntax
    {
        public virtual string QueryFormat(string tableName, IEnumerable<string> fields, IEnumerable<string> joins, string where, string orderBy, int skip, int take)
        {
            var sql = new StringBuilder($"select {string.Join(", ", fields)} from {tableName}", 200);

            if (joins != null)
                sql.AppendFormat(" {0}", string.Join(" ", joins));

            if (where != null)
                sql.AppendFormat(" where {0}", where);

            if (orderBy != null)
                sql.AppendFormat(" {0}", orderBy);

            return sql.ToString();
        }

        public virtual string InsertFormat(string tableName, IEnumerable<string> fields, IEnumerable<string> paramNames, bool hasIdentityKey)
        {
            return $"insert into {tableName}({string.Join(", ", fields)}) values({string.Join(", ", paramNames)})";
        }

        public virtual string DeleteFormat(string tableName, IEnumerable<string> deleteTableAlias, IEnumerable<string> joins, string where)
        {
            var sql = new StringBuilder($"delete {string.Join(", ", deleteTableAlias)} from {tableName}", 200);

            if (joins != null)
                sql.AppendFormat(" {0}", string.Join(" ", joins));

            if (where != null)
                sql.AppendFormat(" where {0}", where);

            return sql.ToString();
        }

        public virtual string UpdateFormat(string tableName, IEnumerable<string> updateFields, string where)
        {
            var sql = new StringBuilder($"update {tableName} set {string.Join(", ", updateFields)}", 200);

            if (where != null)
                sql.AppendFormat(" where {0}", where);

            return sql.ToString();
        }

        public virtual string Join(string tableName, JoinType joinType, string on)
        {
            string joinWay = null;
            switch (joinType)
            {
                case JoinType.Left:
                    joinWay = "left ";
                    break;
                case JoinType.Right:
                    joinWay = "right ";
                    break;
            }

            if (on == null)
                return $"{joinWay}join {tableName}";
            else
                return $"{joinWay}join {tableName} on {on}";
        }

        public virtual string EscapeTableName(string name)
        {
            return name;
        }

        public virtual string TableNameAlias(DbAlias alias)
        {
            return $"{alias.Name} {alias.Alias}";
        }

        public virtual string EscapePropertyName(string name)
        {
            return name;
        }

        public virtual string PropertyNameAlias(DbAlias alias)
        {
            if (alias.Name.Equals(alias.Alias, StringComparison.Ordinal))
                return EscapePropertyName(alias.Name);

            return $"{EscapePropertyName(alias.Name)} {alias.Alias}";
        }

        public virtual string ParameterName(string name)
        {
            return $"@{name}";
        }

        public virtual string Operator(OperatorType operatorType)
        {
            switch (operatorType)
            {
                case OperatorType.AndAlso:
                    return " and ";
                case OperatorType.OrElse:
                    return " or ";
                case OperatorType.Equal:
                    return " = ";
                case OperatorType.NotEqual:
                    return " <> ";
                case OperatorType.GreaterThan:
                    return " > ";
                case OperatorType.GreaterThanOrEqual:
                    return " >= ";
                case OperatorType.LessThan:
                    return " < ";
                case OperatorType.LessThanOrEqual:
                    return " <= ";
                case OperatorType.Add:
                    return " + ";
                case OperatorType.Subtract:
                    return " - ";
                case OperatorType.Multiply:
                    return " * ";
                case OperatorType.Divide:
                    return " / ";
                default:
                    throw new NotImplementedException($"{(ExpressionType)operatorType}");
            }
        }

        public virtual string OrderBy(IEnumerable<string> orderBy, SortType orderBySortType, IEnumerable<string> thenBy, SortType? thenBySortType)
        {
            if (thenBy == null)
                return $"order by {string.Join(", ", orderBy)} {orderBySortType}";

            return $"order by {string.Join(", ", orderBy)} {orderBySortType}, {string.Join(", ", thenBy)} {thenBySortType}";
        }
    }
}
