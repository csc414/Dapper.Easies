using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Easies
{
    public interface IDbQuery
    {
        internal QueryContext Context { get; }
    }

    public interface IDbQuery<T> : IDbQuery, ISelectedQuery<T>, IAggregateQuery<T>
    {
        ISelectedQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> selector);

        IDbQuery<T> Where(Expression<Predicate<T>> predicate);

        IDbQuery<T, TJoin> Join<TJoin>(Expression<Predicate<T, TJoin>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IOrderedDbQuery<T> OrderBy(params Expression<Func<T, object>>[] orderField);

        IOrderedDbQuery<T> OrderByDescending(params Expression<Func<T, object>>[] orderField);
    }

    public interface IDbQuery<T1, T2> : IDbQuery, ISelectedQuery<T1>, IAggregateQuery<T1, T2>
    {
        ISelectedQuery<TResult> Select<TResult>(Expression<Func<T1, T2, TResult>> selector);

        IDbQuery<T1, T2> Where(Expression<Predicate<T1, T2>> predicate);

        IDbQuery<T1, T2, TJoin> Join<TJoin>(Expression<Predicate<T1, T2, TJoin>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IOrderedDbQuery<T1, T2> OrderBy(params Expression<Func<T1, T2, object>>[] orderField);

        IOrderedDbQuery<T1, T2> OrderByDescending(params Expression<Func<T1, T2, object>>[] orderField);
    }

    public interface IDbQuery<T1, T2, T3> : IDbQuery, ISelectedQuery<T1>, IAggregateQuery<T1, T2, T3>
    {
        ISelectedQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, TResult>> selector);

        IDbQuery<T1, T2, T3> Where(Expression<Predicate<T1, T2, T3>> predicate);

        IDbQuery<T1, T2, T3, TJoin> Join<TJoin>(Expression<Predicate<T1, T2, T3, TJoin>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IOrderedDbQuery<T1, T2, T3> OrderBy(params Expression<Func<T1, T2, T3, object>>[] orderField);

        IOrderedDbQuery<T1, T2, T3> OrderByDescending(params Expression<Func<T1, T2, T3, object>>[] orderField);
    }

    public interface IDbQuery<T1, T2, T3, T4> : IDbQuery, ISelectedQuery<T1>, IAggregateQuery<T1, T2, T3, T4>
    {
        ISelectedQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> selector);

        IDbQuery<T1, T2, T3, T4> Where(Expression<Predicate<T1, T2, T3, T4>> predicate);

        IDbQuery<T1, T2, T3, T4, TJoin> Join<TJoin>(Expression<Predicate<T1, T2, T3, T4, TJoin>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IOrderedDbQuery<T1, T2, T3, T4> OrderBy(params Expression<Func<T1, T2, T3, T4, object>>[] orderField);

        IOrderedDbQuery<T1, T2, T3, T4> OrderByDescending(params Expression<Func<T1, T2, T3, T4, object>>[] orderField);
    }

    public interface IDbQuery<T1, T2, T3, T4, T5> : IDbQuery, ISelectedQuery<T1>, IAggregateQuery<T1, T2, T3, T4, T5>
    {
        ISelectedQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> selector);

        IDbQuery<T1, T2, T3, T4, T5> Where(Expression<Predicate<T1, T2, T3, T4, T5>> predicate);

        IOrderedDbQuery<T1, T2, T3, T4, T5> OrderBy(params Expression<Func<T1, T2, T3, T4, T5, object>>[] orderField);

        IOrderedDbQuery<T1, T2, T3, T4, T5> OrderByDescending(params Expression<Func<T1, T2, T3, T4, T5, object>>[] orderField);
    }
}
