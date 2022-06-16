using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Easies
{
    public abstract class DbQuery : IDbQuery
    {
        internal static MethodInfo _expressionMethodInfo = typeof(DbFunc).GetTypeInfo().DeclaredMethods.First(o => o.Name == "Expr").MakeGenericMethod(typeof(string));

        internal static Expression CreateExpressionLambda(LambdaExpression expression)
        {
            return Expression.Lambda(Expression.Call(_expressionMethodInfo, expression.Body), expression.Parameters);
        }

        internal readonly QueryContext _context;

        internal DbQuery(QueryContext context)
        {
            _context = context;
        }

        QueryContext IDbQuery.Context => _context;

        protected Task<T> InternalExecuteAsync<T>(Func<IDbConnection, Task<T>> func) => _context.Connection.ExecuteAsync(_context.DbObject.ConnectionFactory, func);

        protected void AddWhereExpression(Expression whereExpression)
        {
            _context.AddWhere(whereExpression);
        }

        protected void AddHavingExpression(Expression havingExpression)
        {
            _context.AddHaving(havingExpression);
        }

        protected void AddJoinMetedata<TJoin>(Expression joinExpression, JoinType type)
        {
            _context.AddJoin(typeof(TJoin), joinExpression, type);
        }

        protected void AddJoinMetedata<TJoin>(IDbQuery query, Expression joinExpression, JoinType type)
        {
            _context.AddJoin(typeof(TJoin), joinExpression, type, query);
        }

        protected void SetOrderBy(Expression orderFields, SortType sortType)
        {
            if (orderFields == null)
                throw new ArgumentException("排序字段不能为空");

            _context.OrderByMetedata = new OrderByMetedata(orderFields, sortType);
            _context.ThenByMetedata = null;
        }

        protected void SetThenBy(Expression orderFields, SortType sortType)
        {
            if (orderFields == null)
                throw new ArgumentException("排序字段不能为空");

            _context.ThenByMetedata = new OrderByMetedata(orderFields, sortType);
        }

        public Task<long> CountAsync()
        {
            return CountAsync(null);
        }

        protected Task<long> CountAsync(Expression field)
        {
            return InternalExecuteAsync(conn => conn.ExecuteScalarAsync<long>(_context.Converter.ToQuerySql(_context, out var parameters, aggregateInfo: new AggregateInfo(AggregateType.Count, field)), parameters));
        }

        protected Task<TResult> MaxAsync<TResult>(Expression field)
        {
            if (field == null)
                throw new ArgumentException("字段不能为空");

            return InternalExecuteAsync(conn => conn.ExecuteScalarAsync<TResult>(_context.Converter.ToQuerySql(_context, out var parameters, aggregateInfo: new AggregateInfo(AggregateType.Max, field)), parameters));
        }

        protected Task<TResult> MinAsync<TResult>(Expression field)
        {
            if (field == null)
                throw new ArgumentException("字段不能为空");

            return InternalExecuteAsync(conn => conn.ExecuteScalarAsync<TResult>(_context.Converter.ToQuerySql(_context, out var parameters, aggregateInfo: new AggregateInfo(AggregateType.Min, field)), parameters));
        }

        protected Task<decimal> AvgAsync<TResult>(Expression field)
        {
            if (field == null)
                throw new ArgumentException("字段不能为空");

            return InternalExecuteAsync(conn => conn.ExecuteScalarAsync<decimal>(_context.Converter.ToQuerySql(_context, out var parameters, aggregateInfo: new AggregateInfo(AggregateType.Avg, field)), parameters));
        }

        protected Task<decimal> SumAsync<TResult>(Expression field)
        {
            if (field == null)
                throw new ArgumentException("字段不能为空");

            return InternalExecuteAsync(conn => conn.ExecuteScalarAsync<decimal>(_context.Converter.ToQuerySql(_context, out var parameters, aggregateInfo: new AggregateInfo(AggregateType.Sum, field)), parameters));
        }
    }

    public class DbQuery<T> : DbQuery, IDbQuery<T>, IOrderedDbQuery<T>, ISelectedDbQuery<T>, IGroupingDbQuery<T>, IGroupingSelectedDbQuery<T>, IGroupingOrderedDbQuery<T>
    {
        internal DbQuery(QueryContext context) : base(context) { }

        public IDbQuery<T> Where(Expression<Func<T, bool>> predicate)
        {
            AddWhereExpression(predicate);
            return this;
        }

        public IDbQuery<T> Where(Expression<Func<T, string>> expression)
        {
            AddWhereExpression(CreateExpressionLambda(expression));
            return this;
        }

        public ISelectedDbQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
        {
            _context.SelectorExpression = selector;
            return new DbQuery<TResult>(_context);
        }

        public ISelectedDbQuery<TResult> Select<TResult>()
        {
            _context.SelectorExpression = new SelectTypeExpression(typeof(TResult));
            return new DbQuery<TResult>(_context);
        }

        public IDbQuery<T, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, JoinType type = JoinType.Inner) where TJoin : class
        {
            AddJoinMetedata<TJoin>(query, null, type);
            return new DbQuery<T, TJoin>(_context);
        }

        public IDbQuery<T, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, Expression<Func<T, TJoin, bool>> on, JoinType type = JoinType.Inner) where TJoin : class
        {
            AddJoinMetedata<TJoin>(query, on, type);
            return new DbQuery<T, TJoin>(_context);
        }

        public IDbQuery<T, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, Expression<Func<T, TJoin, string>> on, JoinType type = JoinType.Inner) where TJoin : class
        {
            AddJoinMetedata<TJoin>(query, CreateExpressionLambda(on), type);
            return new DbQuery<T, TJoin>(_context);
        }

        public IDbQuery<T, TJoin> Join<TJoin>(JoinType type = JoinType.Inner) where TJoin : IDbObject
        {
            AddJoinMetedata<TJoin>(null, type);
            return new DbQuery<T, TJoin>(_context);
        }

        public IDbQuery<T, TJoin> Join<TJoin>(Expression<Func<T, TJoin, bool>> on, JoinType type = JoinType.Inner) where TJoin : IDbObject
        {
            AddJoinMetedata<TJoin>(on, type);
            return new DbQuery<T, TJoin>(_context);
        }

        public IDbQuery<T, TJoin> Join<TJoin>(Expression<Func<T, TJoin, string>> on, JoinType type = JoinType.Inner) where TJoin : IDbObject
        {
            AddJoinMetedata<TJoin>(CreateExpressionLambda(on), type);
            return new DbQuery<T, TJoin>(_context);
        }

        public IOrderedDbQuery<T> OrderBy(Expression<Func<T, object>> orderFields)
        {
            SetOrderBy(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T> OrderByDescending(Expression<Func<T, object>> orderFields)
        {
            SetOrderBy(orderFields, SortType.Desc);
            return this;
        }

        public IOrderedDbQuery<T> ThenBy(Expression<Func<T, object>> orderFields)
        {
            SetThenBy(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T> ThenByDescending(Expression<Func<T, object>> orderFields)
        {
            SetThenBy(orderFields, SortType.Desc);
            return this;
        }

        public Task<T> FirstAsync()
        {
            return InternalExecuteAsync(conn => conn.QueryFirstAsync<T>(_context.Converter.ToQuerySql(_context, out var parameters, skip: 0, take: 1), parameters));
        }

        public Task<T> FirstOrDefaultAsync()
        {
            return InternalExecuteAsync(conn => conn.QueryFirstOrDefaultAsync<T>(_context.Converter.ToQuerySql(_context, out var parameters, skip: 0, take: 1), parameters));
        }

        public Task<TResult> FirstAsync<TResult>() where TResult : ITuple
        {
            return InternalExecuteAsync(conn => conn.QueryFirstAsync<TResult>(_context.Converter.ToQuerySql(_context, out var parameters, skip: 0, take: 1), parameters));
        }

        public Task<TResult> FirstOrDefaultAsync<TResult>() where TResult : ITuple
        {
            return InternalExecuteAsync(conn => conn.QueryFirstOrDefaultAsync<TResult>(_context.Converter.ToQuerySql(_context, out var parameters, skip: 0, take: 1), parameters));
        }

        public Task<IEnumerable<T>> QueryAsync()
        {
            return InternalExecuteAsync(conn => conn.QueryAsync<T>(_context.Converter.ToQuerySql(_context, out var parameters), parameters));
        }

        public Task<IEnumerable<TResult>> QueryAsync<TResult>() where TResult : ITuple
        {
            return InternalExecuteAsync(conn => conn.QueryAsync<TResult>(_context.Converter.ToQuerySql(_context, out var parameters), parameters));
        }

        public Task<(IEnumerable<T> data, long total, int max_page)> GetPagerAsync(int page, int size) => InternalGetPagerAsync<T>(page, size);

        public Task<(IEnumerable<TResult> data, long total, int max_page)> GetPagerAsync<TResult>(int page, int size) where TResult : ITuple => InternalGetPagerAsync<TResult>(page, size);

        async Task<(IEnumerable<TResult> data, long total, int max_page)> InternalGetPagerAsync<TResult>(int page, int size)
        {
            var total = await CountAsync();
            var max_page = Convert.ToInt32(Math.Ceiling(total * 1f / size));
            if (page > max_page)
                return (Enumerable.Empty<TResult>(), total, max_page);

            var data = await InternalExecuteAsync(conn => conn.QueryAsync<TResult>(_context.Converter.ToQuerySql(_context, out var parameters, skip: (page - 1) * size, take: size), parameters));
            return (data, total, max_page);
        }

        public Task<(IEnumerable<T> data, long total)> GetLimitAsync(int skip, int take) => InternalGetLimitAsync<T>(skip, take);

        public Task<(IEnumerable<TResult> data, long total)> GetLimitAsync<TResult>(int skip, int take) where TResult : ITuple => InternalGetLimitAsync<TResult>(skip, take);

        async Task<(IEnumerable<TResult> data, long total)> InternalGetLimitAsync<TResult>(int skip, int take)
        {
            var total = await CountAsync();
            var data = await InternalExecuteAsync(conn => conn.QueryAsync<TResult>(_context.Converter.ToQuerySql(_context, out var parameters, skip: skip, take: take), parameters));
            return (data, total);
        }

        public async Task<bool> ExistAsync()
        {
            return await CountAsync() > 0;
        }

        public Task<int> DeleteAsync()
        {
            return InternalExecuteAsync(conn => conn.ExecuteAsync(_context.Converter.ToDeleteSql(_context, out var parameters), parameters));
        }

        public Task<int> UpdateAsync(Expression<Func<T>> updateFields) => InternalUpdateAsync(updateFields);

        public Task<int> UpdateAsync(Expression<Func<T, T>> updateFields) => InternalUpdateAsync(updateFields);

        Task<int> InternalUpdateAsync(Expression updateFields)
        {
            var sql = _context.Converter.ToUpdateFieldsSql(updateFields, _context, out var parameters);
            return InternalExecuteAsync(conn => conn.ExecuteAsync(sql, parameters));
        }

        public Task<long> CountAsync(Expression<Func<T, object>> field) => base.CountAsync(field);

        public Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> field) => base.MaxAsync<TResult>(field);

        public Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> field) => base.MinAsync<TResult>(field);

        public Task<decimal> AvgAsync<TField>(Expression<Func<T, TField>> field) => base.AvgAsync<TField>(field);

        public Task<decimal> SumAsync<TField>(Expression<Func<T, TField>> field) => base.SumAsync<TField>(field);

        // ----------------------------------------------- Group By -----------------------------------------------
        public IGroupingDbQuery<T> GroupBy<TFields>(Expression<Func<T, TFields>> fields)
        {
            _context.GroupByExpression = fields;
            _context.OrderByMetedata = null;
            _context.ThenByMetedata = null;
            return this;
        }

        IGroupingSelectedDbQuery<TResult> IGroupingDbQuery<T>.Select<TResult>(Expression<Func<T, TResult>> selector)
        {
            _context.SelectorExpression = selector;
            return new DbQuery<TResult>(_context);
        }

        IGroupingOrderedDbQuery<T> IGroupingSelectedDbQuery<T>.OrderBy(Expression<Func<T, object>> orderFields)
        {
            SetOrderBy(orderFields, SortType.Asc);
            return this;
        }

        IGroupingOrderedDbQuery<T> IGroupingSelectedDbQuery<T>.OrderByDescending(Expression<Func<T, object>> orderFields)
        {
            SetOrderBy(orderFields, SortType.Desc);
            return this;
        }

        IGroupingOrderedDbQuery<T> IGroupingOrderedDbQuery<T>.ThenBy(Expression<Func<T, object>> orderFields)
        {
            SetThenBy(orderFields, SortType.Asc);
            return this;
        }

        IGroupingOrderedDbQuery<T> IGroupingOrderedDbQuery<T>.ThenByDescending(Expression<Func<T, object>> orderFields)
        {
            SetThenBy(orderFields, SortType.Desc);
            return this;
        }

        IGroupingDbQuery<T> IGroupingDbQuery<T>.Having(Expression<Func<T, bool>> predicate)
        {
            AddHavingExpression(predicate);
            return this;
        }

        IGroupingDbQuery<T> IGroupingDbQuery<T>.Having(Expression<Func<T, string>> expression)
        {
            AddHavingExpression(CreateExpressionLambda(expression));
            return this;
        }
    }

    public class DbQuery<T1, T2> : DbQuery<T1>, IDbQuery<T1, T2>, IOrderedDbQuery<T1, T2>, IGroupingDbQuery<T1, T2>
    {
        internal DbQuery(QueryContext context) : base(context) { }

        public IDbQuery<T1, T2> Where(Expression<Func<T1, T2, bool>> predicate)
        {
            AddWhereExpression(predicate);
            return this;
        }

        public IDbQuery<T1, T2> Where(Expression<Func<T1, T2, string>> expression)
        {
            AddWhereExpression(CreateExpressionLambda(expression));
            return this;
        }

        public ISelectedDbQuery<TResult> Select<TResult>(Expression<Func<T1, T2, TResult>> selector)
        {
            _context.SelectorExpression = selector;
            return new DbQuery<TResult>(_context);
        }

        public new IDbQuery<T1, T2, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, JoinType type = JoinType.Inner) where TJoin : class
        {
            AddJoinMetedata<TJoin>(query, null, type);
            return new DbQuery<T1, T2, TJoin>(_context);
        }

        public IDbQuery<T1, T2, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, Expression<Func<T1, T2, TJoin, bool>> on, JoinType type = JoinType.Inner) where TJoin : class
        {
            AddJoinMetedata<TJoin>(query, on, type);
            return new DbQuery<T1, T2, TJoin>(_context);
        }

        public IDbQuery<T1, T2, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, Expression<Func<T1, T2, TJoin, string>> on, JoinType type = JoinType.Inner) where TJoin : class
        {
            AddJoinMetedata<TJoin>(query, CreateExpressionLambda(on), type);
            return new DbQuery<T1, T2, TJoin>(_context);
        }

        public new IDbQuery<T1, T2, TJoin> Join<TJoin>(JoinType type = JoinType.Inner) where TJoin : IDbObject
        {
            AddJoinMetedata<TJoin>(null, type);
            return new DbQuery<T1, T2, TJoin>(_context);
        }

        public IDbQuery<T1, T2, TJoin> Join<TJoin>(Expression<Func<T1, T2, TJoin, bool>> on, JoinType type = JoinType.Inner) where TJoin : IDbObject
        {
            AddJoinMetedata<TJoin>(on, type);
            return new DbQuery<T1, T2, TJoin>(_context);
        }

        public IDbQuery<T1, T2, TJoin> Join<TJoin>(Expression<Func<T1, T2, TJoin, string>> on, JoinType type = JoinType.Inner) where TJoin : IDbObject
        {
            AddJoinMetedata<TJoin>(CreateExpressionLambda(on), type);
            return new DbQuery<T1, T2, TJoin>(_context);
        }

        public IOrderedDbQuery<T1, T2> OrderBy(Expression<Func<T1, T2, object>> orderFields)
        {
            SetOrderBy(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T1, T2> OrderByDescending(Expression<Func<T1, T2, object>> orderFields)
        {
            SetOrderBy(orderFields, SortType.Desc);
            return this;
        }

        public IOrderedDbQuery<T1, T2> ThenBy(Expression<Func<T1, T2, object>> orderFields)
        {
            SetThenBy(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T1, T2> ThenByDescending(Expression<Func<T1, T2, object>> orderFields)
        {
            SetThenBy(orderFields, SortType.Desc);
            return this;
        }

        public Task<long> CountAsync(Expression<Func<T1, T2, object>> field) => base.CountAsync(field);

        public Task<TResult> MaxAsync<TResult>(Expression<Func<T1, T2, TResult>> field) => base.MaxAsync<TResult>(field);

        public Task<TResult> MinAsync<TResult>(Expression<Func<T1, T2, TResult>> field) => base.MinAsync<TResult>(field);

        public Task<decimal> AvgAsync<TField>(Expression<Func<T1, T2, TField>> field) => base.AvgAsync<TField>(field);

        public Task<decimal> SumAsync<TField>(Expression<Func<T1, T2, TField>> field) => base.SumAsync<TField>(field);

        // ----------------------------------------------- Group By -----------------------------------------------
        public IGroupingDbQuery<T1, T2> GroupBy<TFields>(Expression<Func<T1, T2, TFields>> fields)
        {
            _context.GroupByExpression = fields;
            _context.OrderByMetedata = null;
            _context.ThenByMetedata = null;
            return this;
        }

        IGroupingSelectedDbQuery<TResult> IGroupingDbQuery<T1, T2>.Select<TResult>(Expression<Func<T1, T2, TResult>> selector)
        {
            _context.SelectorExpression = selector;
            return new DbQuery<TResult>(_context);
        }

        IGroupingDbQuery<T1, T2> IGroupingDbQuery<T1, T2>.Having(Expression<Func<T1, T2, bool>> predicate)
        {
            AddHavingExpression(predicate);
            return this;
        }

        IGroupingDbQuery<T1, T2> IGroupingDbQuery<T1, T2>.Having(Expression<Func<T1, T2, string>> expression)
        {
            AddHavingExpression(CreateExpressionLambda(expression));
            return this;
        }
    }

    public class DbQuery<T1, T2, T3> : DbQuery<T1, T2>, IDbQuery<T1, T2, T3>, IOrderedDbQuery<T1, T2, T3>, IGroupingDbQuery<T1, T2, T3>
    {
        internal DbQuery(QueryContext context) : base(context) { }

        public IDbQuery<T1, T2, T3> Where(Expression<Func<T1, T2, T3, bool>> predicate)
        {
            AddWhereExpression(predicate);
            return this;
        }

        public IDbQuery<T1, T2, T3> Where(Expression<Func<T1, T2, T3, string>> expression)
        {
            AddWhereExpression(CreateExpressionLambda(expression));
            return this;
        }

        public ISelectedDbQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, TResult>> selector)
        {
            _context.SelectorExpression = selector;
            return new DbQuery<TResult>(_context);
        }

        public new IDbQuery<T1, T2, T3, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, JoinType type = JoinType.Inner) where TJoin : class
        {
            AddJoinMetedata<TJoin>(query, null, type);
            return new DbQuery<T1, T2, T3, TJoin>(_context);
        }

        public IDbQuery<T1, T2, T3, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, Expression<Func<T1, T2, T3, TJoin, bool>> on, JoinType type = JoinType.Inner) where TJoin : class
        {
            AddJoinMetedata<TJoin>(query, on, type);
            return new DbQuery<T1, T2, T3, TJoin>(_context);
        }

        public IDbQuery<T1, T2, T3, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, Expression<Func<T1, T2, T3, TJoin, string>> on, JoinType type = JoinType.Inner) where TJoin : class
        {
            AddJoinMetedata<TJoin>(query, CreateExpressionLambda(on), type);
            return new DbQuery<T1, T2, T3, TJoin>(_context);
        }

        public new IDbQuery<T1, T2, T3, TJoin> Join<TJoin>(JoinType type = JoinType.Inner) where TJoin : IDbObject
        {
            AddJoinMetedata<TJoin>(null, type);
            return new DbQuery<T1, T2, T3, TJoin>(_context);
        }

        public IDbQuery<T1, T2, T3, TJoin> Join<TJoin>(Expression<Func<T1, T2, T3, TJoin, bool>> on, JoinType type = JoinType.Inner) where TJoin : IDbObject
        {
            AddJoinMetedata<TJoin>(on, type);
            return new DbQuery<T1, T2, T3, TJoin>(_context);
        }

        public IDbQuery<T1, T2, T3, TJoin> Join<TJoin>(Expression<Func<T1, T2, T3, TJoin, string>> on, JoinType type = JoinType.Inner) where TJoin : IDbObject
        {
            AddJoinMetedata<TJoin>(CreateExpressionLambda(on), type);
            return new DbQuery<T1, T2, T3, TJoin>(_context);
        }

        public IOrderedDbQuery<T1, T2, T3> OrderBy(Expression<Func<T1, T2, T3, object>> orderFields)
        {
            SetOrderBy(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3> OrderByDescending(Expression<Func<T1, T2, T3, object>> orderFields)
        {
            SetOrderBy(orderFields, SortType.Desc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3> ThenBy(Expression<Func<T1, T2, T3, object>> orderFields)
        {
            SetThenBy(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3> ThenByDescending(Expression<Func<T1, T2, T3, object>> orderFields)
        {
            SetThenBy(orderFields, SortType.Desc);
            return this;
        }

        public Task<long> CountAsync(Expression<Func<T1, T2, T3, object>> field) => base.CountAsync(field);

        public Task<TResult> MaxAsync<TResult>(Expression<Func<T1, T2, T3, TResult>> field) => base.MaxAsync<TResult>(field);

        public Task<TResult> MinAsync<TResult>(Expression<Func<T1, T2, T3, TResult>> field) => base.MinAsync<TResult>(field);

        public Task<decimal> AvgAsync<TField>(Expression<Func<T1, T2, T3, TField>> field) => base.AvgAsync<TField>(field);

        public Task<decimal> SumAsync<TField>(Expression<Func<T1, T2, T3, TField>> field) => base.SumAsync<TField>(field);

        // ----------------------------------------------- Group By -----------------------------------------------
        public IGroupingDbQuery<T1, T2, T3> GroupBy<TFields>(Expression<Func<T1, T2, T3, TFields>> fields)
        {
            _context.GroupByExpression = fields;
            _context.OrderByMetedata = null;
            _context.ThenByMetedata = null;
            return this;
        }

        IGroupingSelectedDbQuery<TResult> IGroupingDbQuery<T1, T2, T3>.Select<TResult>(Expression<Func<T1, T2, T3, TResult>> selector)
        {
            _context.SelectorExpression = selector;
            return new DbQuery<TResult>(_context);
        }

        IGroupingDbQuery<T1, T2, T3> IGroupingDbQuery<T1, T2, T3>.Having(Expression<Func<T1, T2, T3, bool>> predicate)
        {
            AddHavingExpression(predicate);
            return this;
        }

        IGroupingDbQuery<T1, T2, T3> IGroupingDbQuery<T1, T2, T3>.Having(Expression<Func<T1, T2, T3, string>> expression)
        {
            AddHavingExpression(CreateExpressionLambda(expression));
            return this;
        }
    }

    public class DbQuery<T1, T2, T3, T4> : DbQuery<T1, T2, T3>, IDbQuery<T1, T2, T3, T4>, IOrderedDbQuery<T1, T2, T3, T4>, IGroupingDbQuery<T1, T2, T3, T4>
    {
        internal DbQuery(QueryContext context) : base(context) { }

        public IDbQuery<T1, T2, T3, T4> Where(Expression<Func<T1, T2, T3, T4, bool>> predicate)
        {
            AddWhereExpression(predicate);
            return this;
        }

        public IDbQuery<T1, T2, T3, T4> Where(Expression<Func<T1, T2, T3, T4, string>> expression)
        {
            AddWhereExpression(CreateExpressionLambda(expression));
            return this;
        }

        public ISelectedDbQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> selector)
        {
            _context.SelectorExpression = selector;
            return new DbQuery<TResult>(_context);
        }

        public new IDbQuery<T1, T2, T3, T4, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, JoinType type = JoinType.Inner) where TJoin : class
        {
            AddJoinMetedata<TJoin>(query, null, type);
            return new DbQuery<T1, T2, T3, T4, TJoin>(_context);
        }

        public IDbQuery<T1, T2, T3, T4, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, Expression<Func<T1, T2, T3, T4, TJoin, bool>> on, JoinType type = JoinType.Inner) where TJoin : class
        {
            AddJoinMetedata<TJoin>(query, on, type);
            return new DbQuery<T1, T2, T3, T4, TJoin>(_context);
        }

        public IDbQuery<T1, T2, T3, T4, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, Expression<Func<T1, T2, T3, T4, TJoin, string>> on, JoinType type = JoinType.Inner) where TJoin : class
        {
            AddJoinMetedata<TJoin>(query, CreateExpressionLambda(on), type);
            return new DbQuery<T1, T2, T3, T4, TJoin>(_context);
        }

        public new IDbQuery<T1, T2, T3, T4, TJoin> Join<TJoin>(JoinType type = JoinType.Inner) where TJoin : IDbObject
        {
            AddJoinMetedata<TJoin>(null, type);
            return new DbQuery<T1, T2, T3, T4, TJoin>(_context);
        }

        public IDbQuery<T1, T2, T3, T4, TJoin> Join<TJoin>(Expression<Func<T1, T2, T3, T4, TJoin, bool>> on, JoinType type = JoinType.Inner) where TJoin : IDbObject
        {
            AddJoinMetedata<TJoin>(on, type);
            return new DbQuery<T1, T2, T3, T4, TJoin>(_context);
        }

        public IDbQuery<T1, T2, T3, T4, TJoin> Join<TJoin>(Expression<Func<T1, T2, T3, T4, TJoin, string>> on, JoinType type = JoinType.Inner) where TJoin : IDbObject
        {
            AddJoinMetedata<TJoin>(CreateExpressionLambda(on), type);
            return new DbQuery<T1, T2, T3, T4, TJoin>(_context);
        }

        public IOrderedDbQuery<T1, T2, T3, T4> OrderBy(Expression<Func<T1, T2, T3, T4, object>> orderFields)
        {
            SetOrderBy(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3, T4> OrderByDescending(Expression<Func<T1, T2, T3, T4, object>> orderFields)
        {
            SetOrderBy(orderFields, SortType.Desc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3, T4> ThenBy(Expression<Func<T1, T2, T3, T4, object>> orderFields)
        {
            SetThenBy(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3, T4> ThenByDescending(Expression<Func<T1, T2, T3, T4, object>> orderFields)
        {
            SetThenBy(orderFields, SortType.Desc);
            return this;
        }

        public Task<long> CountAsync(Expression<Func<T1, T2, T3, T4, object>> field) => base.CountAsync(field);

        public Task<TResult> MaxAsync<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> field) => base.MaxAsync<TResult>(field);

        public Task<TResult> MinAsync<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> field) => base.MinAsync<TResult>(field);

        public Task<decimal> AvgAsync<TField>(Expression<Func<T1, T2, T3, T4, TField>> field) => base.AvgAsync<TField>(field);

        public Task<decimal> SumAsync<TField>(Expression<Func<T1, T2, T3, T4, TField>> field) => base.SumAsync<TField>(field);

        // ----------------------------------------------- Group By -----------------------------------------------
        public IGroupingDbQuery<T1, T2, T3, T4> GroupBy<TFields>(Expression<Func<T1, T2, T3, T4, TFields>> fields)
        {
            _context.GroupByExpression = fields;
            _context.OrderByMetedata = null;
            _context.ThenByMetedata = null;
            return this;
        }

        IGroupingSelectedDbQuery<TResult> IGroupingDbQuery<T1, T2, T3, T4>.Select<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> selector)
        {
            _context.SelectorExpression = selector;
            return new DbQuery<TResult>(_context);
        }

        IGroupingDbQuery<T1, T2, T3, T4> IGroupingDbQuery<T1, T2, T3, T4>.Having(Expression<Func<T1, T2, T3, T4, bool>> predicate)
        {
            AddHavingExpression(predicate);
            return this;
        }

        IGroupingDbQuery<T1, T2, T3, T4> IGroupingDbQuery<T1, T2, T3, T4>.Having(Expression<Func<T1, T2, T3, T4, string>> expression)
        {
            AddHavingExpression(CreateExpressionLambda(expression));
            return this;
        }
    }

    public class DbQuery<T1, T2, T3, T4, T5> : DbQuery<T1, T2, T3, T4>, IDbQuery<T1, T2, T3, T4, T5>, IOrderedDbQuery<T1, T2, T3, T4, T5>, IGroupingDbQuery<T1, T2, T3, T4, T5>
    {
        internal DbQuery(QueryContext context) : base(context) { }

        public IDbQuery<T1, T2, T3, T4, T5> Where(Expression<Func<T1, T2, T3, T4, T5, bool>> predicate)
        {
            AddWhereExpression(predicate);
            return this;
        }

        public IDbQuery<T1, T2, T3, T4, T5> Where(Expression<Func<T1, T2, T3, T4, T5, string>> expression)
        {
            AddWhereExpression(CreateExpressionLambda(expression));
            return this;
        }

        public ISelectedDbQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> selector)
        {
            _context.SelectorExpression = selector;
            return new DbQuery<TResult>(_context);
        }

        public IOrderedDbQuery<T1, T2, T3, T4, T5> OrderBy(Expression<Func<T1, T2, T3, T4, T5, object>> orderFields)
        {
            SetOrderBy(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3, T4, T5> OrderByDescending(Expression<Func<T1, T2, T3, T4, T5, object>> orderFields)
        {
            SetOrderBy(orderFields, SortType.Desc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3, T4, T5> ThenBy(Expression<Func<T1, T2, T3, T4, T5, object>> orderFields)
        {
            SetThenBy(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3, T4, T5> ThenByDescending(Expression<Func<T1, T2, T3, T4, T5, object>> orderFields)
        {
            SetThenBy(orderFields, SortType.Desc);
            return this;
        }

        public Task<long> CountAsync(Expression<Func<T1, T2, T3, T4, T5, object>> field) => base.CountAsync(field);

        public Task<TResult> MaxAsync<TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> field) => base.MaxAsync<TResult>(field);

        public Task<TResult> MinAsync<TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> field) => base.MinAsync<TResult>(field);

        public Task<decimal> AvgAsync<TField>(Expression<Func<T1, T2, T3, T4, T5, TField>> field) => base.AvgAsync<TField>(field);

        public Task<decimal> SumAsync<TField>(Expression<Func<T1, T2, T3, T4, T5, TField>> field) => base.SumAsync<TField>(field);

        // ----------------------------------------------- Group By -----------------------------------------------
        public IGroupingDbQuery<T1, T2, T3, T4, T5> GroupBy<TFields>(Expression<Func<T1, T2, T3, T4, T5, TFields>> fields)
        {
            _context.GroupByExpression = fields;
            _context.OrderByMetedata = null;
            _context.ThenByMetedata = null;
            return this;
        }

        IGroupingSelectedDbQuery<TResult> IGroupingDbQuery<T1, T2, T3, T4, T5>.Select<TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> selector)
        {
            _context.SelectorExpression = selector;
            return new DbQuery<TResult>(_context);
        }

        IGroupingDbQuery<T1, T2, T3, T4, T5> IGroupingDbQuery<T1, T2, T3, T4, T5>.Having(Expression<Func<T1, T2, T3, T4, T5, bool>> predicate)
        {
            AddHavingExpression(predicate);
            return this;
        }

        IGroupingDbQuery<T1, T2, T3, T4, T5> IGroupingDbQuery<T1, T2, T3, T4, T5>.Having(Expression<Func<T1, T2, T3, T4, T5, string>> expression)
        {
            AddHavingExpression(CreateExpressionLambda(expression));
            return this;
        }
    }
}
