using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Dapper.Easies
{
    public class DefaultSqlSyntax : ISqlSyntax
    {
        public virtual string SelectFormat(string tableName, IEnumerable<string> fields, IEnumerable<string> joins, string where, string groupBy, string having, string orderBy, int skip, int take, bool distinct)
        {
            var sql = new StringBuilder($"SELECT");
            if (distinct)
                sql.AppendFormat(" DISTINCT");

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

            return sql.ToString();
        }

        public virtual string InsertFormat(string tableName, IEnumerable<string> fields, IEnumerable<string> paramNames, bool hasIdentityKey)
        {
            return $"INSERT INTO {tableName}({string.Join(", ", fields)}) VALUES({string.Join(", ", paramNames)})";
        }

        public virtual string DeleteFormat(string tableName, string tableAlias, IEnumerable<string> joins, string where)
        {
            var sql = new StringBuilder("DELETE");
            if (tableAlias != null)
                sql.Append($" {tableAlias}");

            sql.Append($" FROM {tableName} {tableAlias}");

            if (joins != null)
                sql.AppendFormat(" {0}", string.Join(" ", joins));

            if (where != null)
                sql.AppendFormat(" WHERE {0}", where);

            return sql.ToString();
        }

        public virtual string UpdateFormat(string tableName, string tableAlias, IEnumerable<string> updateFields, string where)
        {
            var sql = new StringBuilder($"UPDATE {tableName}");
            if (tableAlias != null)
                sql.Append($" {tableAlias}");

            sql.Append($" SET {string.Join(", ", updateFields)}");
            if (where != null)
                sql.AppendFormat(" WHERE {0}", where);

            return sql.ToString();
        }

        public virtual string Join(string tableName, JoinType joinType, string on)
        {
            string joinWay = null;
            switch (joinType)
            {
                case JoinType.Left:
                    joinWay = "LEFT ";
                    break;
                case JoinType.Right:
                    joinWay = "RIGHT ";
                    break;
            }

            if (on == null)
                return $"{joinWay}JOIN {tableName}";
            else
                return $"{joinWay}JOIN {tableName} ON {on}";
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
            //表达式
            if (alias.IsExpr)
                return $"{alias.Name} AS {EscapePropertyName(alias.Alias)}";

            return $"{EscapePropertyName(alias.Name)} AS {EscapePropertyName(alias.Alias)}";
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
                    return " AND ";
                case OperatorType.OrElse:
                    return " OR ";
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
                case OperatorType.EqualNull:
                    return " IS NULL";
                case OperatorType.NotEqualNull:
                    return " IS NOT NULL";
                case OperatorType.Not:
                    return "NOT ";
                default:
                    return null;
            }
        }

        public virtual string OrderBy(IEnumerable<string> orderBy, SortType orderBySortType, IEnumerable<string> thenBy, SortType? thenBySortType)
        {
            if (thenBy == null)
                return $"ORDER BY {string.Join(", ", orderBy)} {orderBySortType}";

            return $"ORDER BY {string.Join(", ", orderBy)} {orderBySortType}, {string.Join(", ", thenBy)} {thenBySortType}";
        }

        public virtual string Method(string methodName, Expression[] args, ParameterBuilder parameter, Func<Expression, string> getExpr, Func<Expression, object> getValue)
        {
            switch (methodName)
            {
                case "Like":
                    return $"{getExpr(args[0])} LIKE {getExpr(args[1])}";
                case "In":
                    return $"{getExpr(args[0])} IN {getExpr(args[1])}";
                case "NotIn":
                    return $"{getExpr(args[0])} NOT IN {getExpr(args[1])}";
                case "Count":
                    var field = args.FirstOrDefault();
                    return $"COUNT({ (field == null ? "*" : getExpr(field))})";
                case "Min":
                    return $"MIN({getExpr(args[0])})";
                case "Max":
                    return $"MAX({getExpr(args[0])})";
                case "Avg":
                    return $"AVG({getExpr(args[0])})";
                case "Sum":
                    return $"SUM({getExpr(args[0])})";
                case "IsNull":
                    return $"{getExpr(args[0])} IS NULL";
                case "IsNotNull":
                    return $"{getExpr(args[0])} IS NOT NULL";
                case "N":
                    return getExpr(args[0]);
                default:
                    return null;
            }
        }

        public virtual string DateTimeMethod(string name, Func<string> getPropertyName)
        {
            switch (name)
            {
                case "Year":
                    return $"YEAR({getPropertyName()})";
                case "Month":
                    return $"MONTH({getPropertyName()})";
                case "Day":
                    return $"DAY({getPropertyName()})";
                default:
                    return null;
            }
        }

        public string GroupBy(IEnumerable<string> groupBy)
        {
            return $"GROUP BY {string.Join(", ", groupBy)}";
        }
    }
}
