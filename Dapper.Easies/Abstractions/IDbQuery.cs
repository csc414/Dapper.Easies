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

        IDbQuery<T> Where(Expression<Func<T, bool>> predicate);

        IDbQuery<T> Where(Expression<Func<T, string>> expression);

        IDbQuery<T, TJoin> Join<TJoin>(Expression<Func<T, TJoin, bool>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IDbQuery<T, TJoin> Join<TJoin>(Expression<Func<T, TJoin, string>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IOrderedDbQuery<T> OrderBy<TField>(params Expression<Func<T, TField>>[] orderField);

        IOrderedDbQuery<T> OrderByDescending<TField>(params Expression<Func<T, TField>>[] orderField);
    }

    public interface IDbQuery<T1, T2> : IDbQuery, ISelectedQuery<T1>, IAggregateQuery<T1, T2>
    {
        ISelectedQuery<TResult> Select<TResult>(Expression<Func<T1, T2, TResult>> selector);

        IDbQuery<T1, T2> Where(Expression<Func<T1, T2, bool>> predicate);

        IDbQuery<T1, T2> Where(Expression<Func<T1, T2, string>> expression);

        IDbQuery<T1, T2, TJoin> Join<TJoin>(Expression<Func<T1, T2, TJoin, bool>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IDbQuery<T1, T2, TJoin> Join<TJoin>(Expression<Func<T1, T2, TJoin, string>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IOrderedDbQuery<T1, T2> OrderBy<TField>(params Expression<Func<T1, T2, TField>>[] orderField);

        IOrderedDbQuery<T1, T2> OrderByDescending<TField>(params Expression<Func<T1, T2, TField>>[] orderField);
    }

    public interface IDbQuery<T1, T2, T3> : IDbQuery, ISelectedQuery<T1>, IAggregateQuery<T1, T2, T3>
    {
        ISelectedQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, TResult>> selector);

        IDbQuery<T1, T2, T3> Where(Expression<Func<T1, T2, T3, bool>> predicate);

        IDbQuery<T1, T2, T3> Where(Expression<Func<T1, T2, T3, string>> expression);

        IDbQuery<T1, T2, T3, TJoin> Join<TJoin>(Expression<Func<T1, T2, T3, TJoin, bool>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IDbQuery<T1, T2, T3, TJoin> Join<TJoin>(Expression<Func<T1, T2, T3, TJoin, string>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IOrderedDbQuery<T1, T2, T3> OrderBy<TField>(params Expression<Func<T1, T2, T3, TField>>[] orderField);

        IOrderedDbQuery<T1, T2, T3> OrderByDescending<TField>(params Expression<Func<T1, T2, T3, TField>>[] orderField);
    }

    public interface IDbQuery<T1, T2, T3, T4> : IDbQuery, ISelectedQuery<T1>, IAggregateQuery<T1, T2, T3, T4>
    {
        ISelectedQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> selector);

        IDbQuery<T1, T2, T3, T4> Where(Expression<Func<T1, T2, T3, T4, bool>> predicate);

        IDbQuery<T1, T2, T3, T4> Where(Expression<Func<T1, T2, T3, T4, string>> expression);

        IDbQuery<T1, T2, T3, T4, TJoin> Join<TJoin>(Expression<Func<T1, T2, T3, T4, TJoin, bool>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IDbQuery<T1, T2, T3, T4, TJoin> Join<TJoin>(Expression<Func<T1, T2, T3, T4, TJoin, string>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IOrderedDbQuery<T1, T2, T3, T4> OrderBy<TField>(params Expression<Func<T1, T2, T3, T4, TField>>[] orderField);

        IOrderedDbQuery<T1, T2, T3, T4> OrderByDescending<TField>(params Expression<Func<T1, T2, T3, T4, TField>>[] orderField);
    }

    public interface IDbQuery<T1, T2, T3, T4, T5> : IDbQuery, ISelectedQuery<T1>, IAggregateQuery<T1, T2, T3, T4, T5>
    {
        ISelectedQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> selector);

        IDbQuery<T1, T2, T3, T4, T5> Where(Expression<Func<T1, T2, T3, T4, T5, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5> Where(Expression<Func<T1, T2, T3, T4, T5, string>> expression);

        IOrderedDbQuery<T1, T2, T3, T4, T5> OrderBy<TField>(params Expression<Func<T1, T2, T3, T4, T5, TField>>[] orderField);

        IOrderedDbQuery<T1, T2, T3, T4, T5> OrderByDescending<TField>(params Expression<Func<T1, T2, T3, T4, T5, TField>>[] orderField);
    }
}
