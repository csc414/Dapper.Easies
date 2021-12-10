using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Easies
{
    public abstract class DbQuery
    {
        internal readonly QueryContext _context;

        internal DbQuery(QueryContext context)
        {
            _context = context;
        }

        protected void AddWhereExpression(Expression whereExpression)
        {
            _context.WhereExpressions.Add(whereExpression);
        }

        protected void AddJoinMetedata<TJoin>(Expression joinExpression, JoinType type)
        {
            _context.JoinMetedatas.Add(new JoinMetedata { DbTable = typeof(TJoin), JoinExpression = joinExpression, Type = type });
        }

        public Task<T> FirstAsync<T>()
        {
            return Task.FromResult<T>(default);
        }

        public Task<T> FirstOrDefaultAsync<T>()
        {
            return Task.FromResult<T>(default);
        }
    }

    public class DbQuery<T> : DbQuery
    {
        internal DbQuery(QueryContext context) : base(context) { }

        public DbQuery<T> Where(Expression<Predicate<T>> predicate)
        {
            AddWhereExpression(predicate);
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

        public DbQuery<T, TJoin> Join<TJoin>(Expression<Predicate<T, TJoin>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbTable
        {
            AddJoinMetedata<TJoin>(on, type);
            return new DbQuery<T, TJoin>(_context);
        }
    }

    public class DbQuery<T1, T2> : DbQuery<T1>
    {
        internal DbQuery(QueryContext context) : base(context) { }

        public DbQuery<T1, T2> Where(Expression<Predicate<T1, T2>> predicate)
        {
            AddWhereExpression(predicate);
            return this;
        }

        public DbQuery<T1, T2, TJoin> Join<TJoin>(Expression<Predicate<T1, T2, TJoin>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbTable
        {
            AddJoinMetedata<TJoin>(on, type);
            return new DbQuery<T1, T2, TJoin>(_context);
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

        public DbQuery<T1, T2, T3, TJoin> Join<TJoin>(Expression<Predicate<T1, T2, T3, TJoin>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbTable
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

        public DbQuery<T1, T2, T3, T4, TJoin> Join<TJoin>(Expression<Predicate<T1, T2, T3, T4, TJoin>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbTable
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
