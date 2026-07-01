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
            // schema-only 全表查询快路径：无聚合/无投影/无 distinct/无 join/无 where(含 appender)/无 group/无 having/无 order/无分页。
            // 此类 SQL 仅依赖表结构，首次生成后缓存复用，跳过 parser 构造与字段遍历。
            if (aggregateInfo == null
                && context.SelectorExpression == null
                && !context.Distinct
                && context.JoinMetedatas == null
                && context.WhereExpressions == null
                && context.GroupByExpression == null
                && context.HavingExpressions == null
                && context.OrderByMetedata == null
                && (take ?? context.Take) == 0)
            {
                var cached = context.DbObject.CachedSelectAllSql;
                if (cached != null)
                    return cached;
                return BuildAndCacheSelectAll(context);
            }

            var parser = new MySqlExpressionParser(context);
            var sql = new StringBuilder("SELECT", 0x100);
            var alias = context.Alias[0];

            if (context.Distinct)
                sql.AppendFormat(" DISTINCT");

            bool wrapCount = false;

            if (aggregateInfo != null)
            {
                switch (aggregateInfo.Type)
                {
                    case AggregateType.Count:
                        if (context.GroupByExpression != null)
                        {
                            sql.Append(" 1");
                            wrapCount = true;
                        }
                        else
                        {
                            sql.Append(" COUNT(");
                            if (aggregateInfo.Expression == null)
                                sql.Append('*');
                        }
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
                if (!wrapCount)
                {
                    if (aggregateInfo.Expression != null)
                        parser.Visit(aggregateInfo.Expression, sql, parameterBuilder);
                    sql.Append(")");
                }
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

            sql.Append(" FROM ");
            sql.Append(AliasTableName(alias.Name, alias.Alias));

            AppendJoin(parser, context, sql, parameterBuilder);

            AppendWhere(parser, context, sql, parameterBuilder);

            AppendGroup(parser, context, sql, parameterBuilder);

            AppendHaving(parser, context, sql, parameterBuilder);

            if (aggregateInfo == null)
                AppendSort(parser, context, sql, parameterBuilder);

            var takeCount = take ?? context.Take;
            if (takeCount > 0)
            {
                sql.Append(" LIMIT ").Append(skip ?? context.Skip).Append(',').Append(takeCount);
            }

            if (wrapCount)
                return "SELECT COUNT(*) FROM (" + sql.ToString() + ") _t";

            return sql.ToString();
        }

        private string BuildAndCacheSelectAll(QueryContext context)
        {
            var alias = context.Alias[0];
            var sql = new StringBuilder(0x100);
            sql.Append("SELECT ");
            var i = 0;
            foreach (var property in context.DbObject.Properties)
            {
                if (i > 0)
                    sql.Append(", ");
                sql.Append(alias.Alias).Append('.').Append(property.EscapeNameAsAlias);
                i++;
            }
            sql.Append(" FROM ").Append(AliasTableName(alias.Name, alias.Alias));

            var result = sql.ToString();
            context.DbObject.CachedSelectAllSql = result;
            return result;
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
            var primaryKeys = dbObject.PrimaryKeys;
            if (primaryKeys.Count == 0)
                throw new ArgumentException("实体类没有主键");

            if (ids.Length < primaryKeys.Count)
                throw new ArgumentException("参数与主键数不一致");

            // 仅 where 部分含参数；select/from/limit 结构稳定，缓存模板后只追加参数化 where。
            var sql = new StringBuilder(dbObject.CachedGetByIdSql ?? BuildGetByIdSql(dbObject), 0x100);
            var i = 0;
            foreach (var item in primaryKeys)
            {
                if (i > 0)
                    sql.Append(" AND ");
                sql.Append(item.EscapeName).Append(" = ").Append(parameterBuilder.Add(ids[i]));
                i++;
            }
            sql.Append(" LIMIT 0,1");
            return sql.ToString();
        }

        private string BuildGetByIdSql(DbObject dbObject)
        {
            var sql = new StringBuilder("SELECT ", 0x80);
            var i = 0;
            foreach (var item in dbObject.Properties)
            {
                if (i > 0)
                    sql.Append(", ");
                sql.Append(item.EscapeNameAsAlias);
                i++;
            }
            sql.Append(" FROM ").Append(dbObject.EscapeName).Append(" WHERE ");
            var cache = sql.ToString();
            dbObject.CachedGetByIdSql = cache;
            return cache;
        }

        public virtual string InsertFormat(DbObject dbObject, bool hasIdentityKey)
        {
            var cached = dbObject.CachedInsertSql;
            if (cached != null)
                return cached;

            var sql = new StringBuilder(0x80);
            sql.Append("INSERT INTO ").Append(dbObject.EscapeName).Append('(');
            var properties = dbObject.NonIdentityProperties;
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
                sql.Append('@').Append(item.PropertyInfo.Name);
                i++;
            }
            sql.Append(')');

            if (hasIdentityKey)
                sql.Append("; SELECT LAST_INSERT_ID()");

            var result = sql.ToString();
            dbObject.CachedInsertSql = result;
            return result;
        }

        public virtual string DeleteFormat(QueryContext context, ParameterBuilder parameterBuilder)
        {
            var parser = new MySqlExpressionParser(context);
            var alias = context.Alias[0];
            var sql = new StringBuilder(0x80);
            sql.Append("DELETE ").Append(alias.Alias).Append(" FROM ").Append(AliasTableName(alias.Name, alias.Alias));

            AppendWhere(parser, context, sql, parameterBuilder);

            return sql.ToString();
        }

        public virtual string DeleteFormat(DbObject dbObject)
        {
            var cached = dbObject.CachedDeleteSql;
            if (cached != null)
                return cached;

            var primaryKeys = dbObject.PrimaryKeys;
            if (primaryKeys.Count == 0)
                throw new ArgumentException("实体类没有主键");

            var sql = new StringBuilder(0x40);
            sql.Append("DELETE FROM ").Append(dbObject.EscapeName);

            sql.Append(" WHERE ");
            var i = 0;
            foreach (var item in primaryKeys)
            {
                if (i > 0)
                    sql.Append(" AND ");
                sql.Append(item.EscapeName).Append(" = @").Append(item.PropertyInfo.Name);
                i++;
            }
            var result = sql.ToString();
            dbObject.CachedDeleteSql = result;
            return result;
        }

        public virtual string UpdateFormat(Expression fields, QueryContext context, ParameterBuilder parameterBuilder)
        {
            var parser = new MySqlExpressionParser(context);
            var alias = context.Alias[0];
            var sql = new StringBuilder(0x80);
            sql.Append("UPDATE ").Append(AliasTableName(alias.Name, alias.Alias)).Append(" SET ");

            parser.VisitFields(fields, sql, parameterBuilder, updateMode: true);

            AppendWhere(parser, context, sql, parameterBuilder);

            return sql.ToString();
        }

        public virtual string UpdateFormat(DbObject dbObject)
        {
            var cached = dbObject.CachedUpdateSql;
            if (cached != null)
                return cached;

            var primaryKeys = dbObject.PrimaryKeys;
            if (primaryKeys.Count == 0)
                throw new ArgumentException("实体类没有主键");

            var sql = new StringBuilder(0x80);
            sql.Append("UPDATE ").Append(dbObject.EscapeName).Append(" SET ");

            var i = 0;
            foreach (var item in dbObject.NonPrimaryKeyProperties)
            {
                if (i > 0)
                    sql.Append(", ");
                sql.Append(item.EscapeName).Append(" = @").Append(item.PropertyInfo.Name);
                i++;
            }

            sql.Append(" WHERE ");
            i = 0;
            foreach (var item in primaryKeys)
            {
                if (i > 0)
                    sql.Append(" AND ");
                sql.Append(item.EscapeName).Append(" = @").Append(item.PropertyInfo.Name);
                i++;
            }
            var result = sql.ToString();
            dbObject.CachedUpdateSql = result;
            return result;
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
