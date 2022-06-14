using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies.MySql
{
    public class MySqlSqlSyntax : ISqlSyntax
    {
        internal static MySqlSqlSyntax Instance { get; } = new MySqlSqlSyntax();

        public virtual string SelectFormat(QueryContext context, ParameterBuilder parameterBuilder, int? skip = null, int? take = null, AggregateInfo aggregateInfo = null)
        {
            var sqlSyntax = context.DbObject.SqlSyntax;
            var parser = new MySqlExpressionParser(context);
            var sql = new StringBuilder("SELECT", 0x100);
            if (context.Distinct)
                sql.AppendFormat(" DISTINCT");

            var alias = context.Alias[0];

            if (aggregateInfo != null)
            {
                switch (aggregateInfo.Type)
                {
                    case AggregateType.Count:
                        sql.Append(" COUNT(");
                        if (aggregateInfo.Expression == null)
                            sql.Append('*');
                        break;
                    case AggregateType.Max:
                        sql.Append(" MAX(");
                        break;
                    case AggregateType.Min:
                        sql.Append(" MIN(");
                        break;
                    case AggregateType.Avg:
                        sql.Append(" AVG(");
                        break;
                    case AggregateType.Sum:
                        sql.Append(" SUM(");
                        break;
                }
                if (aggregateInfo.Expression != null)
                    parser.Visit(aggregateInfo.Expression, sql, parameterBuilder);
                sql.Append(")");
            }
            else if (context.SelectorExpression != null)
            {
                var lambda = (LambdaExpression)context.SelectorExpression;
                sql.Append(' ');
                if (lambda.Body is ParameterExpression parameter)
                {
                    var aliasIndex = lambda.Parameters.IndexOf(parameter);
                    var specificDbObject = DbObject.Get(parameter.Type);
                    if (specificDbObject == null)
                    {
                        var tableAlias = context.Alias[aliasIndex];
                        var i = 0;
                        foreach (var property in parameter.Type.GetProperties())
                        {
                            if (i > 0)
                                sql.Append(", ");
                            sql.Append($"{tableAlias.Alias}.{sqlSyntax.EscapePropertyName(property.Name)}");
                            i++;
                        }
                    }
                    else
                    {
                        var i = 0;
                        foreach (var property in specificDbObject.Properties)
                        {
                            if (i > 0)
                                sql.Append(", ");
                            sql.Append($"{alias.Alias}.{property.EscapeNameAsAlias}");
                            i++;
                        }
                    }
                }
                else
                    parser.VisitFields(context.SelectorExpression, sql, parameterBuilder);
            }
            else
            {
                var i = 0;
                foreach (var property in context.DbObject.Properties)
                {
                    if (i > 0)
                        sql.Append(", ");
                    sql.Append($"{alias.Alias}.{property.EscapeNameAsAlias}");
                    i++;
                }
            }

            sql.Append(" FROM ");
            sql.Append(AliasTableName(alias.Name, alias.Alias));

            if (context.JoinMetedatas != null)
            {
                var i = 1;
                foreach (var join in context.JoinMetedatas)
                {
                    var joinAlias = context.Alias[i];
                    string tableName;
                    if (join.DbObject == null)
                        tableName = sqlSyntax.AliasTableName($"({join.Query.Context.Converter.ToQuerySql(join.Query.Context, parameterBuilder)})", joinAlias.Alias);
                    else
                        tableName = sqlSyntax.AliasTableName(joinAlias.Name, joinAlias.Alias);

                    switch (join.Type)
                    {
                        case JoinType.Inner:
                            sql.Append(" JOIN ");
                            break;
                        case JoinType.Left:
                            sql.Append(" LEFT JOIN ");
                            break;
                        case JoinType.Right:
                            sql.Append(" RIGHT JOIN ");
                            break;
                    }

                    sql.Append(tableName);
                    if (join.JoinExpression != null)
                    {
                        sql.Append(" ON ");
                        parser.Visit(join.JoinExpression, sql, parameterBuilder);
                    }

                    i++;
                }
            }

            if (context.WhereExpressions != null)
            {
                sql.Append(" WHERE ");
                parser.Visit(context.WhereExpressions, sql, parameterBuilder);
            }

            if (context.GroupByExpression != null)
            {
                sql.Append(" GROUP BY ");
                parser.VisitFields(context.GroupByExpression, sql, parameterBuilder, hasAlias: false);
            }

            if(context.HavingExpressions != null)
            {
                sql.Append(" HAVING ");
                parser.Visit(context.HavingExpressions, sql, parameterBuilder);
            }

            if(context.OrderByMetedata != null)
            {
                sql.Append(" ORDER BY ");
                parser.VisitFields(context.OrderByMetedata.Expression, sql, parameterBuilder, hasAlias: false);

                if (context.OrderByMetedata.SortType == SortType.Asc)
                    sql.Append(" ASC");
                else
                    sql.Append(" DESC");

                if (context.ThenByMetedata != null)
                {
                    sql.Append(", ");
                    parser.VisitFields(context.ThenByMetedata.Expression, sql, parameterBuilder, hasAlias: false);
                    if (context.ThenByMetedata.SortType == SortType.Asc)
                        sql.Append(" ASC");
                    else
                        sql.Append(" DESC");
                }
            }
            return sql.ToString();
        }

        //string GetOrderBy(QueryContext context, ParameterBuilder builder)
        //{
        //    if (context.OrderByMetedata == null)
        //        return null;

        //    var orderBy = GetOrderByFields(sqlSyntax, context, builder, context.OrderByMetedata);
        //    if (context.ThenByMetedata == null)
        //        return sqlSyntax.OrderBy(orderBy, context.OrderByMetedata.SortType, null, null);

        //    var thenBy = GetOrderByFields(sqlSyntax, context, builder, context.ThenByMetedata);
        //    return sqlSyntax.OrderBy(orderBy, context.OrderByMetedata.SortType, thenBy, context.ThenByMetedata.SortType);
        //}

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

        //public virtual string Join(string tableName, JoinType joinType, string on)
        //{
        //    string joinWay = null;
        //    switch (joinType)
        //    {
        //        case JoinType.Left:
        //            joinWay = "LEFT ";
        //            break;
        //        case JoinType.Right:
        //            joinWay = "RIGHT ";
        //            break;
        //    }

        //    if (on == null)
        //        return $"{joinWay}JOIN {tableName}";
        //    else
        //        return $"{joinWay}JOIN {tableName} ON {on}";
        //}

        public virtual string EscapeTableName(string name)
        {
            return $"`{name}`";
        }

        public virtual string EscapePropertyName(string name)
        {
            return $"`{name}`";
        }

        public virtual string AliasTableName(string name, string alias)
        {
            return $"{name} {alias}";
        }

        public virtual string AliasPropertyName(string name, string alias)
        {
            return $"{name} {alias}";
        }

        //public virtual string OrderBy(IEnumerable<string> orderBy, SortType orderBySortType, IEnumerable<string> thenBy, SortType? thenBySortType)
        //{
        //    if (thenBy == null)
        //        return $"ORDER BY {string.Join(", ", orderBy)} {orderBySortType}";

        //    return $"ORDER BY {string.Join(", ", orderBy)} {orderBySortType}, {string.Join(", ", thenBy)} {thenBySortType}";
        //}

        //public string GroupBy(IEnumerable<string> groupBy)
        //{
        //    return $"GROUP BY {string.Join(", ", groupBy)}";
        //}
    }
}
