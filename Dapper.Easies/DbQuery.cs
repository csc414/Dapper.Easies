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
                throw new ArgumentException($"Already join {dbObject.Type.Name}.");

            _context.JoinMetedatas.Add(new JoinMetedata { DbObject = dbObject, JoinExpression = joinExpression, Type = type });
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

        public Task<T> FirstAsync<T>()
        {
            _context.Take = 1;
            return _context.Connection.QueryFirstAsync<T>(_context.Converter.ToQuerySql(_context, out var parameters), parameters);
        }

        public Task<T> FirstOrDefaultAsync<T>()
        {
            _context.Take = 1;
            return _context.Connection.QueryFirstOrDefaultAsync<T>(_context.Converter.ToQuerySql(_context, out var parameters), parameters);
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
            return FirstAsync<T>();
        }

        public Task<T> FirstOrDefaultAsync()
        {
            return FirstOrDefaultAsync<T>();
        }
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

        public DbQuery<T1, T2, TJoin> Join<TJoin>(Expression<Predicate<T1, T2, TJoin>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject
        {
            AddJoinMetedata<TJoin>(on, type);
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
    }

    public class DbQuery<T1, T2, T3> : DbQuery<T1, T2>
    {
        internal DbQuery(QueryContext context) : base(context) { }

        public DbQuery<T1, T2, T3> Where(Expression<Predicate<T1, T2, T3>> predicate)
        {
            AddWhereExpression(predicate);
            return this;
        }

        public DbQuery<T1, T2, T3, TJoin> Join<TJoin>(Expression<Predicate<T1, T2, T3, TJoin>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject
        {
            AddJoinMetedata<TJoin>(on, type);
            return new DbQuery<T1, T2, T3, TJoin>(_context);
        }
    }

    public class DbQuery<T1, T2, T3, T4> : DbQuery<T1, T2, T3>
    {
        internal DbQuery(QueryContext context) : base(context) { }

        public DbQuery<T1, T2, T3, T4> Where(Expression<Predicate<T1, T2, T3, T4>> predicate)
        {
            AddWhereExpression(predicate);
            return this;
        }

        public DbQuery<T1, T2, T3, T4, TJoin> Join<TJoin>(Expression<Predicate<T1, T2, T3, T4, TJoin>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject
        {
            AddJoinMetedata<TJoin>(on, type);
            return new DbQuery<T1, T2, T3, T4, TJoin>(_context);
        }
    }

    public class DbQuery<T1, T2, T3, T4, T5> : DbQuery<T1, T2, T3, T4>
    {
        internal DbQuery(QueryContext context) : base(context) { }

        public DbQuery<T1, T2, T3, T4, T5> Where(Expression<Predicate<T1, T2, T3, T4, T5>> predicate)
        {
            AddWhereExpression(predicate);
            return this;
        }
    }
}
