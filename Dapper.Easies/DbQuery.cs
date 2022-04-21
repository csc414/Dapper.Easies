using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

        protected void SetOrderBy(IEnumerable<Expression> orderFields, SortType sortType)
        {
            if (orderFields == null || !orderFields.Any())
                throw new ArgumentException("排序字段不能为空");

            _context.OrderByMetedata = new OrderByMetedata(orderFields, sortType);
            _context.ThenByMetedata = null;
        }

        protected void SetThenBy(IEnumerable<Expression> orderFields, SortType sortType)
        {
            if (orderFields == null || !orderFields.Any())
                throw new ArgumentException("排序字段不能为空");

            _context.ThenByMetedata = new OrderByMetedata(orderFields, sortType);
        }

        public Task<long> CountAsync()
        {
            return _context.Connection.ExecuteScalarAsync<long>(_context.Converter.ToQuerySql(_context, out var parameters, aggregateInfo: new AggregateInfo(AggregateType.Count, null)), parameters);
        }

        protected Task<long> CountAsync(Expression field)
        {
            return _context.Connection.ExecuteScalarAsync<long>(_context.Converter.ToQuerySql(_context, out var parameters, aggregateInfo: new AggregateInfo(AggregateType.Count, field)), parameters);
        }

        protected Task<TResult> MaxAsync<TResult>(Expression field)
        {
            if (field == null)
                throw new ArgumentException("字段不能为空");

            return _context.Connection.ExecuteScalarAsync<TResult>(_context.Converter.ToQuerySql(_context, out var parameters, aggregateInfo: new AggregateInfo(AggregateType.Max, field)), parameters);
        }

        protected Task<TResult> MinAsync<TResult>(Expression field)
        {
            if (field == null)
                throw new ArgumentException("字段不能为空");

            return _context.Connection.ExecuteScalarAsync<TResult>(_context.Converter.ToQuerySql(_context, out var parameters, aggregateInfo: new AggregateInfo(AggregateType.Min, field)), parameters);
        }

        protected Task<decimal> AvgAsync<TResult>(Expression field)
        {
            if (field == null)
                throw new ArgumentException("字段不能为空");

            return _context.Connection.ExecuteScalarAsync<decimal>(_context.Converter.ToQuerySql(_context, out var parameters, aggregateInfo: new AggregateInfo(AggregateType.Avg, field)), parameters);
        }

        protected Task<decimal> SumAsync<TResult>(Expression field)
        {
            if (field == null)
                throw new ArgumentException("字段不能为空");

            return _context.Connection.ExecuteScalarAsync<decimal>(_context.Converter.ToQuerySql(_context, out var parameters, aggregateInfo: new AggregateInfo(AggregateType.Sum, field)), parameters);
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

        public IOrderedDbQuery<T> OrderBy(params Expression<Func<T, object>>[] orderFields)
        {
            SetOrderBy(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T> OrderByDescending(params Expression<Func<T, object>>[] orderFields)
        {
            SetOrderBy(orderFields, SortType.Desc);
            return this;
        }

        public IOrderedDbQuery<T> ThenBy(params Expression<Func<T, object>>[] orderFields)
        {
            SetThenBy(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T> ThenByDescending(params Expression<Func<T, object>>[] orderFields)
        {
            SetThenBy(orderFields, SortType.Desc);
            return this;
        }

        public Task<T> FirstAsync()
        {
            return _context.Connection.QueryFirstAsync<T>(_context.Converter.ToQuerySql(_context, out var parameters, take: 1), parameters);
        }

        public Task<T> FirstOrDefaultAsync()
        {
            return _context.Connection.QueryFirstOrDefaultAsync<T>(_context.Converter.ToQuerySql(_context, out var parameters, take: 1), parameters);
        }

        public Task<IEnumerable<T>> QueryAsync()
        {
            return _context.Connection.QueryAsync<T>(_context.Converter.ToQuerySql(_context, out var parameters), parameters);
        }

        public async Task<bool> ExistAsync()
        {
            return await base.CountAsync(null) > 0;
        }

        public Task<int> DeleteAsync()
        {
            return _context.Connection.ExecuteAsync(_context.Converter.ToDeleteSql(_context, out var parameters), parameters);
        }

        public Task<int> UpdateAsync(Expression<Func<T>> updateFields) => InternalUpdateAsync(updateFields);

        public Task<int> UpdateAsync(Expression<Func<T, T>> updateFields) => InternalUpdateAsync(updateFields);

        Task<int> InternalUpdateAsync(Expression updateFields)
        {
            var sql = _context.Converter.ToUpdateFieldsSql(updateFields, _context, out var parameters);
            return _context.Connection.ExecuteAsync(sql, parameters);
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

        IGroupingOrderedDbQuery<T> IGroupingSelectedDbQuery<T>.OrderBy(params Expression<Func<T, object>>[] orderFields)
        {
            SetOrderBy(orderFields, SortType.Asc);
            return this;
        }

        IGroupingOrderedDbQuery<T> IGroupingSelectedDbQuery<T>.OrderByDescending(params Expression<Func<T, object>>[] orderFields)
        {
            SetOrderBy(orderFields, SortType.Desc);
            return this;
        }

        IGroupingOrderedDbQuery<T> IGroupingOrderedDbQuery<T>.ThenBy(params Expression<Func<T, object>>[] orderFields)
        {
            SetThenBy(orderFields, SortType.Asc);
            return this;
        }

        IGroupingOrderedDbQuery<T> IGroupingOrderedDbQuery<T>.ThenByDescending(params Expression<Func<T, object>>[] orderFields)
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

    public class DbQuery<T1, T2> : DbQuery<T1>, IDbQuery<T1, T2>, IOrderedDbQuery<T1, T2>, IGroupingDbQuery<T1, T2>, IGroupingSelectedDbQuery<T1, T2>, IGroupingOrderedDbQuery<T1, T2>
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

        public IOrderedDbQuery<T1, T2> OrderBy(params Expression<Func<T1, T2, object>>[] orderFields)
        {
            SetOrderBy(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T1, T2> OrderByDescending(params Expression<Func<T1, T2, object>>[] orderFields)
        {
            SetOrderBy(orderFields, SortType.Desc);
            return this;
        }

        public IOrderedDbQuery<T1, T2> ThenBy(params Expression<Func<T1, T2, object>>[] orderFields)
        {
            SetThenBy(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T1, T2> ThenByDescending(params Expression<Func<T1, T2, object>>[] orderFields)
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

        IGroupingOrderedDbQuery<T1, T2> IGroupingSelectedDbQuery<T1, T2>.OrderBy(params Expression<Func<T1, T2, object>>[] orderFields)
        {
            SetOrderBy(orderFields, SortType.Asc);
            return this;
        }

        IGroupingOrderedDbQuery<T1, T2> IGroupingSelectedDbQuery<T1, T2>.OrderByDescending(params Expression<Func<T1, T2, object>>[] orderFields)
        {
            SetOrderBy(orderFields, SortType.Desc);
            return this;
        }

        IGroupingOrderedDbQuery<T1, T2> IGroupingOrderedDbQuery<T1, T2>.ThenBy(params Expression<Func<T1, T2, object>>[] orderFields)
        {
            SetThenBy(orderFields, SortType.Asc);
            return this;
        }

        IGroupingOrderedDbQuery<T1, T2> IGroupingOrderedDbQuery<T1, T2>.ThenByDescending(params Expression<Func<T1, T2, object>>[] orderFields)
        {
            SetThenBy(orderFields, SortType.Desc);
            return this;
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

    public class DbQuery<T1, T2, T3> : DbQuery<T1, T2>, IDbQuery<T1, T2, T3>, IOrderedDbQuery<T1, T2, T3>, IGroupingDbQuery<T1, T2, T3>, IGroupingSelectedDbQuery<T1, T2, T3>, IGroupingOrderedDbQuery<T1, T2, T3>
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

        public IOrderedDbQuery<T1, T2, T3> OrderBy(params Expression<Func<T1, T2, T3, object>>[] orderFields)
        {
            SetOrderBy(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3> OrderByDescending(params Expression<Func<T1, T2, T3, object>>[] orderFields)
        {
            SetOrderBy(orderFields, SortType.Desc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3> ThenBy(params Expression<Func<T1, T2, T3, object>>[] orderFields)
        {
            SetThenBy(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3> ThenByDescending(params Expression<Func<T1, T2, T3, object>>[] orderFields)
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

        IGroupingOrderedDbQuery<T1, T2, T3> IGroupingSelectedDbQuery<T1, T2, T3>.OrderBy(params Expression<Func<T1, T2, T3, object>>[] orderFields)
        {
            SetOrderBy(orderFields, SortType.Asc);
            return this;
        }

        IGroupingOrderedDbQuery<T1, T2, T3> IGroupingSelectedDbQuery<T1, T2, T3>.OrderByDescending(params Expression<Func<T1, T2, T3, object>>[] orderFields)
        {
            SetOrderBy(orderFields, SortType.Desc);
            return this;
        }

        IGroupingOrderedDbQuery<T1, T2, T3> IGroupingOrderedDbQuery<T1, T2, T3>.ThenBy(params Expression<Func<T1, T2, T3, object>>[] orderFields)
        {
            SetThenBy(orderFields, SortType.Asc);
            return this;
        }

        IGroupingOrderedDbQuery<T1, T2, T3> IGroupingOrderedDbQuery<T1, T2, T3>.ThenByDescending(params Expression<Func<T1, T2, T3, object>>[] orderFields)
        {
            SetThenBy(orderFields, SortType.Desc);
            return this;
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

    public class DbQuery<T1, T2, T3, T4> : DbQuery<T1, T2, T3>, IDbQuery<T1, T2, T3, T4>, IOrderedDbQuery<T1, T2, T3, T4>, IGroupingDbQuery<T1, T2, T3, T4>, IGroupingSelectedDbQuery<T1, T2, T3, T4>, IGroupingOrderedDbQuery<T1, T2, T3, T4>
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

        public IOrderedDbQuery<T1, T2, T3, T4> OrderBy(params Expression<Func<T1, T2, T3, T4, object>>[] orderFields)
        {
            SetOrderBy(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3, T4> OrderByDescending(params Expression<Func<T1, T2, T3, T4, object>>[] orderFields)
        {
            SetOrderBy(orderFields, SortType.Desc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3, T4> ThenBy(params Expression<Func<T1, T2, T3, T4, object>>[] orderFields)
        {
            SetThenBy(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3, T4> ThenByDescending(params Expression<Func<T1, T2, T3, T4, object>>[] orderFields)
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

        IGroupingOrderedDbQuery<T1, T2, T3, T4> IGroupingSelectedDbQuery<T1, T2, T3, T4>.OrderBy(params Expression<Func<T1, T2, T3, T4, object>>[] orderFields)
        {
            SetOrderBy(orderFields, SortType.Asc);
            return this;
        }

        IGroupingOrderedDbQuery<T1, T2, T3, T4> IGroupingSelectedDbQuery<T1, T2, T3, T4>.OrderByDescending(params Expression<Func<T1, T2, T3, T4, object>>[] orderFields)
        {
            SetOrderBy(orderFields, SortType.Desc);
            return this;
        }

        IGroupingOrderedDbQuery<T1, T2, T3, T4> IGroupingOrderedDbQuery<T1, T2, T3, T4>.ThenBy(params Expression<Func<T1, T2, T3, T4, object>>[] orderFields)
        {
            SetThenBy(orderFields, SortType.Asc);
            return this;
        }

        IGroupingOrderedDbQuery<T1, T2, T3, T4> IGroupingOrderedDbQuery<T1, T2, T3, T4>.ThenByDescending(params Expression<Func<T1, T2, T3, T4, object>>[] orderFields)
        {
            SetThenBy(orderFields, SortType.Desc);
            return this;
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

    public class DbQuery<T1, T2, T3, T4, T5> : DbQuery<T1, T2, T3, T4>, IDbQuery<T1, T2, T3, T4, T5>, IOrderedDbQuery<T1, T2, T3, T4, T5>, IGroupingDbQuery<T1, T2, T3, T4, T5>, IGroupingSelectedDbQuery<T1, T2, T3, T4, T5>, IGroupingOrderedDbQuery<T1, T2, T3, T4, T5>
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

        public IOrderedDbQuery<T1, T2, T3, T4, T5> OrderBy(params Expression<Func<T1, T2, T3, T4, T5, object>>[] orderFields)
        {
            SetOrderBy(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3, T4, T5> OrderByDescending(params Expression<Func<T1, T2, T3, T4, T5, object>>[] orderFields)
        {
            SetOrderBy(orderFields, SortType.Desc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3, T4, T5> ThenBy(params Expression<Func<T1, T2, T3, T4, T5, object>>[] orderFields)
        {
            SetThenBy(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3, T4, T5> ThenByDescending(params Expression<Func<T1, T2, T3, T4, T5, object>>[] orderFields)
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

        IGroupingOrderedDbQuery<T1, T2, T3, T4, T5> IGroupingSelectedDbQuery<T1, T2, T3, T4, T5>.OrderBy(params Expression<Func<T1, T2, T3, T4, T5, object>>[] orderFields)
        {
            SetOrderBy(orderFields, SortType.Asc);
            return this;
        }

        IGroupingOrderedDbQuery<T1, T2, T3, T4, T5> IGroupingSelectedDbQuery<T1, T2, T3, T4, T5>.OrderByDescending(params Expression<Func<T1, T2, T3, T4, T5, object>>[] orderFields)
        {
            SetOrderBy(orderFields, SortType.Desc);
            return this;
        }

        IGroupingOrderedDbQuery<T1, T2, T3, T4, T5> IGroupingOrderedDbQuery<T1, T2, T3, T4, T5>.ThenBy(params Expression<Func<T1, T2, T3, T4, T5, object>>[] orderFields)
        {
            SetThenBy(orderFields, SortType.Asc);
            return this;
        }

        IGroupingOrderedDbQuery<T1, T2, T3, T4, T5> IGroupingOrderedDbQuery<T1, T2, T3, T4, T5>.ThenByDescending(params Expression<Func<T1, T2, T3, T4, T5, object>>[] orderFields)
        {
            SetThenBy(orderFields, SortType.Desc);
            return this;
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
