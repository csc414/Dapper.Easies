using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Easies
{
    public abstract class DbQuery : IDbQuery
    {
        internal readonly QueryContext _context;

        internal DbQuery(QueryContext context)
        {
            _context = context;
        }

        QueryContext IDbQuery.Context => _context;

        protected void AddWhereExpression(Expression whereExpression)
        {
            _context.WhereExpressions.Add(whereExpression);
        }

        protected void AddJoinMetedata<TJoin>(Expression joinExpression, JoinType type)
        {
            var dbObject = DbObject.Get(typeof(TJoin));
            if (!_context.Alias.TryAdd(dbObject.Type, new DbAlias(dbObject.EscapeName, $"t{_context.Alias.Count}")))
                throw new ArgumentException($"请勿重复连接表 {dbObject.Type.Name}.");

            _context.JoinMetedatas.Add(new JoinMetedata { DbObject = dbObject, JoinExpression = joinExpression, Type = type });
        }

        protected void SetOrderBy<TField>(IEnumerable<Expression> orderFields, SortType sortType)
        {
            if (orderFields == null || !orderFields.Any())
                throw new ArgumentException("排序字段不能为空");

            _context.OrderByMetedata = new OrderByMetedata(orderFields, sortType);
            _context.ThenByMetedata = null;
        }

        protected void SetThenBy<TField>(IEnumerable<Expression> orderFields, SortType sortType)
        {
            if (orderFields == null || !orderFields.Any())
                throw new ArgumentException("排序字段不能为空");

            _context.ThenByMetedata = new OrderByMetedata(orderFields, sortType);
        }

        protected Task<int> CountAsync(Expression field)
        {
            return _context.Connection.ExecuteScalarAsync<int>(_context.Converter.ToQuerySql(_context, out var parameters, aggregateInfo: new AggregateInfo(AggregateType.Count, field)), parameters);
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
    }

    public class DbQuery<T> : DbQuery, IDbQuery<T>, IOrderedDbQuery<T>, ISelectedQuery<T>
    {
        internal DbQuery(QueryContext context) : base(context) { }

        public IDbQuery<T> Where(Expression<Predicate<T>> predicate)
        {
            AddWhereExpression(predicate);
            return this;
        }

        public ISelectedQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
        {
            _context.SelectorExpression = selector;
            return new DbQuery<TResult>(_context);
        }

        public IDbQuery<T, TJoin> Join<TJoin>(Expression<Predicate<T, TJoin>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject
        {
            AddJoinMetedata<TJoin>(on, type);
            return new DbQuery<T, TJoin>(_context);
        }

        public IOrderedDbQuery<T> OrderBy<TField>(params Expression<Func<T, TField>>[] orderFields)
        {
            SetOrderBy<TField>(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T> OrderByDescending<TField>(params Expression<Func<T, TField>>[] orderFields)
        {
            SetOrderBy<TField>(orderFields, SortType.Desc);
            return this;
        }

        public IOrderedDbQuery<T> ThenBy<TField>(params Expression<Func<T, TField>>[] orderFields)
        {
            SetThenBy<TField>(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T> ThenByDescending<TField>(params Expression<Func<T, TField>>[] orderFields)
        {
            SetThenBy<TField>(orderFields, SortType.Desc);
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

        public Task<int> CountAsync(Expression<Func<T, object>> field) => base.CountAsync(field);

        public Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> field) => base.MaxAsync<TResult>(field);

        public Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> field) => base.MinAsync<TResult>(field);
    }

    public class DbQuery<T1, T2> : DbQuery<T1>, IDbQuery<T1, T2>, IOrderedDbQuery<T1, T2>
    {
        internal DbQuery(QueryContext context) : base(context) { }

        public IDbQuery<T1, T2> Where(Expression<Predicate<T1, T2>> predicate)
        {
            AddWhereExpression(predicate);
            return this;
        }

        public ISelectedQuery<TResult> Select<TResult>(Expression<Func<T1, T2, TResult>> selector)
        {
            _context.SelectorExpression = selector;
            return new DbQuery<TResult>(_context);
        }

        public IDbQuery<T1, T2, TJoin> Join<TJoin>(Expression<Predicate<T1, T2, TJoin>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject
        {
            AddJoinMetedata<TJoin>(on, type);
            return new DbQuery<T1, T2, TJoin>(_context);
        }

        public IOrderedDbQuery<T1, T2> OrderBy<TField>(params Expression<Func<T1, T2, TField>>[] orderFields)
        {
            SetOrderBy<TField>(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T1, T2> OrderByDescending<TField>(params Expression<Func<T1, T2, TField>>[] orderFields)
        {
            SetOrderBy<TField>(orderFields, SortType.Desc);
            return this;
        }

        public IOrderedDbQuery<T1, T2> ThenBy<TField>(params Expression<Func<T1, T2, TField>>[] orderFields)
        {
            SetThenBy<TField>(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T1, T2> ThenByDescending<TField>(params Expression<Func<T1, T2, TField>>[] orderFields)
        {
            SetThenBy<TField>(orderFields, SortType.Desc);
            return this;
        }

        public Task<int> CountAsync(Expression<Func<T1, T2, object>> field) => base.CountAsync(field);

        public Task<TResult> MaxAsync<TResult>(Expression<Func<T1, T2, TResult>> field) => base.MaxAsync<TResult>(field);

        public Task<TResult> MinAsync<TResult>(Expression<Func<T1, T2, TResult>> field) => base.MinAsync<TResult>(field);
    }

    public class DbQuery<T1, T2, T3> : DbQuery<T1, T2>, IDbQuery<T1, T2, T3>, IOrderedDbQuery<T1, T2, T3>
    {
        internal DbQuery(QueryContext context) : base(context) { }

        public IDbQuery<T1, T2, T3> Where(Expression<Predicate<T1, T2, T3>> predicate)
        {
            AddWhereExpression(predicate);
            return this;
        }

        public ISelectedQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, TResult>> selector)
        {
            _context.SelectorExpression = selector;
            return new DbQuery<TResult>(_context);
        }

        public IDbQuery<T1, T2, T3, TJoin> Join<TJoin>(Expression<Predicate<T1, T2, T3, TJoin>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject
        {
            AddJoinMetedata<TJoin>(on, type);
            return new DbQuery<T1, T2, T3, TJoin>(_context);
        }

        public IOrderedDbQuery<T1, T2, T3> OrderBy<TField>(params Expression<Func<T1, T2, T3, TField>>[] orderFields)
        {
            SetOrderBy<TField>(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3> OrderByDescending<TField>(params Expression<Func<T1, T2, T3, TField>>[] orderFields)
        {
            SetOrderBy<TField>(orderFields, SortType.Desc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3> ThenBy<TField>(params Expression<Func<T1, T2, T3, TField>>[] orderFields)
        {
            SetThenBy<TField>(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3> ThenByDescending<TField>(params Expression<Func<T1, T2, T3, TField>>[] orderFields)
        {
            SetThenBy<TField>(orderFields, SortType.Desc);
            return this;
        }

        public Task<int> CountAsync(Expression<Func<T1, T2, T3, object>> field) => base.CountAsync(field);

        public Task<TResult> MaxAsync<TResult>(Expression<Func<T1, T2, T3, TResult>> field) => base.MaxAsync<TResult>(field);

        public Task<TResult> MinAsync<TResult>(Expression<Func<T1, T2, T3, TResult>> field) => base.MinAsync<TResult>(field);
    }

    public class DbQuery<T1, T2, T3, T4> : DbQuery<T1, T2, T3>, IDbQuery<T1, T2, T3, T4>, IOrderedDbQuery<T1, T2, T3, T4>
    {
        internal DbQuery(QueryContext context) : base(context) { }

        public IDbQuery<T1, T2, T3, T4> Where(Expression<Predicate<T1, T2, T3, T4>> predicate)
        {
            AddWhereExpression(predicate);
            return this;
        }

        public ISelectedQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> selector)
        {
            _context.SelectorExpression = selector;
            return new DbQuery<TResult>(_context);
        }

        public IDbQuery<T1, T2, T3, T4, TJoin> Join<TJoin>(Expression<Predicate<T1, T2, T3, T4, TJoin>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject
        {
            AddJoinMetedata<TJoin>(on, type);
            return new DbQuery<T1, T2, T3, T4, TJoin>(_context);
        }

        public IOrderedDbQuery<T1, T2, T3, T4> OrderBy<TField>(params Expression<Func<T1, T2, T3, T4, TField>>[] orderFields)
        {
            SetOrderBy<TField>(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3, T4> OrderByDescending<TField>(params Expression<Func<T1, T2, T3, T4, TField>>[] orderFields)
        {
            SetOrderBy<TField>(orderFields, SortType.Desc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3, T4> ThenBy<TField>(params Expression<Func<T1, T2, T3, T4, TField>>[] orderFields)
        {
            SetThenBy<TField>(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3, T4> ThenByDescending<TField>(params Expression<Func<T1, T2, T3, T4, TField>>[] orderFields)
        {
            SetThenBy<TField>(orderFields, SortType.Desc);
            return this;
        }

        public Task<int> CountAsync(Expression<Func<T1, T2, T3, T4, object>> field) => base.CountAsync(field);

        public Task<TResult> MaxAsync<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> field) => base.MaxAsync<TResult>(field);

        public Task<TResult> MinAsync<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> field) => base.MinAsync<TResult>(field);
    }

    public class DbQuery<T1, T2, T3, T4, T5> : DbQuery<T1, T2, T3, T4>, IDbQuery<T1, T2, T3, T4, T5>, IOrderedDbQuery<T1, T2, T3, T4, T5>
    {
        internal DbQuery(QueryContext context) : base(context) { }

        public IDbQuery<T1, T2, T3, T4, T5> Where(Expression<Predicate<T1, T2, T3, T4, T5>> predicate)
        {
            AddWhereExpression(predicate);
            return this;
        }

        public ISelectedQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> selector)
        {
            _context.SelectorExpression = selector;
            return new DbQuery<TResult>(_context);
        }

        public IOrderedDbQuery<T1, T2, T3, T4, T5> OrderBy<TField>(params Expression<Func<T1, T2, T3, T4, T5, TField>>[] orderFields)
        {
            SetOrderBy<TField>(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3, T4, T5> OrderByDescending<TField>(params Expression<Func<T1, T2, T3, T4, T5, TField>>[] orderFields)
        {
            SetOrderBy<TField>(orderFields, SortType.Desc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3, T4, T5> ThenBy<TField>(params Expression<Func<T1, T2, T3, T4, T5, TField>>[] orderFields)
        {
            SetThenBy<TField>(orderFields, SortType.Asc);
            return this;
        }

        public IOrderedDbQuery<T1, T2, T3, T4, T5> ThenByDescending<TField>(params Expression<Func<T1, T2, T3, T4, T5, TField>>[] orderFields)
        {
            SetThenBy<TField>(orderFields, SortType.Desc);
            return this;
        }

        public Task<int> CountAsync(Expression<Func<T1, T2, T3, T4, T5, object>> field) => base.CountAsync(field);

        public Task<TResult> MaxAsync<TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> field) => base.MaxAsync<TResult>(field);

        public Task<TResult> MinAsync<TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> field) => base.MinAsync<TResult>(field);
    }
}
