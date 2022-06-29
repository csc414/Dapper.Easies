using System;
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
            var parser = new MySqlExpressionParser(context);
            var sql = new StringBuilder("SELECT", 0x100);
            var alias = context.Alias[0];

            if (context.Distinct)
                sql.AppendFormat(" DISTINCT");

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
                sql.Append(' ');
                parser.VisitFields(context.SelectorExpression, sql, parameterBuilder);
            }
            else
            {
                sql.Append(' ');
                parser.VisitFields(context.DbObject, alias, sql);
            }

            sql.Append($" FROM {AliasTableName(alias.Name, alias.Alias)}");

            AppendJoin(parser, context, sql, parameterBuilder);

            AppendWhere(parser, context, sql, parameterBuilder);

            AppendGroup(parser, context, sql, parameterBuilder);

            AppendHaving(parser, context, sql, parameterBuilder);

            if (aggregateInfo == null)
                AppendSort(parser, context, sql, parameterBuilder);

            var takeCount = take ?? context.Take;
            if (takeCount > 0)
                sql.Append($" LIMIT {skip ?? context.Skip},{takeCount}");

            return sql.ToString();
        }

        protected void AppendJoin(MySqlExpressionParser parser, QueryContext context, StringBuilder sql, ParameterBuilder parameterBuilder)
        {
            var sqlSyntax = context.DbObject.SqlSyntax;
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
        }

        protected void AppendWhere(MySqlExpressionParser parser, QueryContext context, StringBuilder sql, ParameterBuilder parameterBuilder)
        {
            if (context.WhereExpressions != null)
            {
                sql.Append(" WHERE ");
                parser.Visit(context.WhereExpressions, sql, parameterBuilder);
            }
        }

        protected void AppendGroup(MySqlExpressionParser parser, QueryContext context, StringBuilder sql, ParameterBuilder parameterBuilder)
        {
            if (context.GroupByExpression != null)
            {
                sql.Append(" GROUP BY ");
                parser.VisitFields(context.GroupByExpression, sql, parameterBuilder, hasAlias: false);
            }
        }

        protected void AppendHaving(MySqlExpressionParser parser, QueryContext context, StringBuilder sql, ParameterBuilder parameterBuilder)
        {
            if (context.HavingExpressions != null)
            {
                sql.Append(" HAVING ");
                parser.Visit(context.HavingExpressions, sql, parameterBuilder);
            }
        }

        protected void AppendSort(MySqlExpressionParser parser, QueryContext context, StringBuilder sql, ParameterBuilder parameterBuilder)
        {
            if (context.OrderByMetedata != null)
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
        }

        public virtual string SelectFormat(DbObject dbObject, object[] ids, ParameterBuilder parameterBuilder)
        {
            var primaryKeys = dbObject.Properties.Where(o => o.PrimaryKey).ToArray();
            if (primaryKeys.Length == 0)
                throw new ArgumentException("实体类没有主键");

            if (ids.Length < primaryKeys.Length)
                throw new ArgumentException("参数与主键数不一致");

            var sql = new StringBuilder("SELECT ", 0x80);
            var i = 0;
            foreach (var item in dbObject.Properties)
            {
                if (i > 0)
                    sql.Append(", ");
                sql.Append(item.EscapeNameAsAlias);
                i++;
            }
            sql.Append($" FROM {dbObject.EscapeName} WHERE ");
            i = 0;
            foreach (var item in primaryKeys)
            {
                if (i > 0)
                    sql.Append(" AND ");
                sql.Append($"{item.EscapeName} = {parameterBuilder.Add(ids[i])}");
                i++;
            }
            sql.Append(" LIMIT 0,1");
            return sql.ToString();
        }

        public virtual string InsertFormat(DbObject dbObject, bool hasIdentityKey)
        {
            var sql = new StringBuilder($"INSERT INTO {dbObject.EscapeName}(", 0x80);
            var properties = dbObject.Properties.Where(o => !o.IdentityKey);
            var i = 0;
            foreach (var item in properties)
            {
                if (i > 0)
                    sql.Append(", ");
                sql.Append(item.EscapeName);
                i++;
            }
            sql.Append(") VALUES(");
            i = 0;
            foreach (var item in properties)
            {
                if (i > 0)
                    sql.Append(", ");
                sql.Append($"@{item.PropertyInfo.Name}");
                i++;
            }
            sql.Append(")");

            if (hasIdentityKey)
                sql.Append("; SELECT LAST_INSERT_ID()");

            return sql.ToString();
        }

        public virtual string DeleteFormat(QueryContext context, ParameterBuilder parameterBuilder)
        {
            var parser = new MySqlExpressionParser(context);
            var alias = context.Alias[0];
            var sql = new StringBuilder($"DELETE {alias.Alias} FROM {AliasTableName(alias.Name, alias.Alias)}", 0x80);

            AppendWhere(parser, context, sql, parameterBuilder);

            return sql.ToString();
        }

        public virtual string DeleteFormat(DbObject dbObject)
        {
            var primaryKeys = dbObject.Properties.Where(o => o.PrimaryKey).ToArray();
            if (primaryKeys.Length == 0)
                throw new ArgumentException("实体类没有主键");

            var sql = new StringBuilder($"DELETE FROM {dbObject.EscapeName}", 0x40);

            sql.Append(" WHERE ");
            var i = 0;
            foreach (var item in primaryKeys)
            {
                if (i > 0)
                    sql.Append(" AND ");
                sql.Append($"{item.EscapeName} = @{item.PropertyInfo.Name}");
                i++;
            }
            return sql.ToString();
        }

        public virtual string UpdateFormat(Expression fields, QueryContext context, ParameterBuilder parameterBuilder)
        {
            var parser = new MySqlExpressionParser(context);
            var alias = context.Alias[0];
            var sql = new StringBuilder($"UPDATE {AliasTableName(alias.Name, alias.Alias)} SET ", 0x80);

            parser.VisitFields(fields, sql, parameterBuilder, updateMode: true);

            AppendWhere(parser, context, sql, parameterBuilder);

            return sql.ToString();
        }

        public virtual string UpdateFormat(DbObject dbObject)
        {
            var primaryKeys = dbObject.Properties.Where(o => o.PrimaryKey).ToArray();
            if (primaryKeys.Length == 0)
                throw new ArgumentException("实体类没有主键");

            var sql = new StringBuilder($"UPDATE {dbObject.EscapeName} SET ", 0x80);

            var i = 0;
            foreach (var item in dbObject.Properties.Where(o => !o.PrimaryKey))
            {
                if (i > 0)
                    sql.Append(", ");
                sql.Append($"{item.EscapeName} = @{item.PropertyInfo.Name}");
                i++;
            }

            sql.Append(" WHERE ");
            i = 0;
            foreach (var item in primaryKeys)
            {
                if (i > 0)
                    sql.Append(" AND ");
                sql.Append($"{item.EscapeName} = @{item.PropertyInfo.Name}");
                i++;
            }
            return sql.ToString();
        }

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

        public virtual string AliasPropertyName(string name, string alias, bool force)
        {
            if (!force && name.Equals(alias, StringComparison.Ordinal))
                return name;

            return $"{name} AS {alias}";
        }
    }
}
