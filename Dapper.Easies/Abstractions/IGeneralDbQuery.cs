using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
    public interface IGeneralDbQuery<T> : IDbQuery, ISelectedDbQuery<T>, IAggregateDbQuery<T>
    {
        ISelectedDbQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> selector);

        IDbQuery<T> Where(Expression<Func<T, bool>> predicate);

        IDbQuery<T> Where(Expression<Func<T, string>> expression);

        IDbQuery<T, TJoin> Join<TJoin>(Expression<Func<T, TJoin, bool>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IDbQuery<T, TJoin> Join<TJoin>(Expression<Func<T, TJoin, string>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IOrderedDbQuery<T> OrderBy<TField>(params Expression<Func<T, TField>>[] orderFields);

        IOrderedDbQuery<T> OrderByDescending<TField>(params Expression<Func<T, TField>>[] orderFields);

        IGroupingDbQuery<T> GroupBy<TFields>(Expression<Func<T, TFields>> fields);
    }

    public interface IGeneralDbQuery<T1, T2> : IDbQuery, ISelectedDbQuery<T1>, IAggregateDbQuery<T1, T2>
    {
        ISelectedDbQuery<TResult> Select<TResult>(Expression<Func<T1, T2, TResult>> selector);

        IDbQuery<T1, T2> Where(Expression<Func<T1, T2, bool>> predicate);

        IDbQuery<T1, T2> Where(Expression<Func<T1, T2, string>> expression);

        IDbQuery<T1, T2, TJoin> Join<TJoin>(Expression<Func<T1, T2, TJoin, bool>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IDbQuery<T1, T2, TJoin> Join<TJoin>(Expression<Func<T1, T2, TJoin, string>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IOrderedDbQuery<T1, T2> OrderBy<TField>(params Expression<Func<T1, T2, TField>>[] orderFields);

        IOrderedDbQuery<T1, T2> OrderByDescending<TField>(params Expression<Func<T1, T2, TField>>[] orderFields);

        IGroupingDbQuery<T1, T2> GroupBy<TFields>(Expression<Func<T1, T2, TFields>> fields);
    }

    public interface IGeneralDbQuery<T1, T2, T3> : IDbQuery, ISelectedDbQuery<T1>, IAggregateDbQuery<T1, T2, T3>
    {
        ISelectedDbQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, TResult>> selector);

        IDbQuery<T1, T2, T3> Where(Expression<Func<T1, T2, T3, bool>> predicate);

        IDbQuery<T1, T2, T3> Where(Expression<Func<T1, T2, T3, string>> expression);

        IDbQuery<T1, T2, T3, TJoin> Join<TJoin>(Expression<Func<T1, T2, T3, TJoin, bool>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IDbQuery<T1, T2, T3, TJoin> Join<TJoin>(Expression<Func<T1, T2, T3, TJoin, string>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IOrderedDbQuery<T1, T2, T3> OrderBy<TField>(params Expression<Func<T1, T2, T3, TField>>[] orderFields);

        IOrderedDbQuery<T1, T2, T3> OrderByDescending<TField>(params Expression<Func<T1, T2, T3, TField>>[] orderFields);

        IGroupingDbQuery<T1, T2, T3> GroupBy<TFields>(Expression<Func<T1, T2, T3, TFields>> fields);
    }

    public interface IGeneralDbQuery<T1, T2, T3, T4> : IDbQuery, ISelectedDbQuery<T1>, IAggregateDbQuery<T1, T2, T3, T4>
    {
        ISelectedDbQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> selector);

        IDbQuery<T1, T2, T3, T4> Where(Expression<Func<T1, T2, T3, T4, bool>> predicate);

        IDbQuery<T1, T2, T3, T4> Where(Expression<Func<T1, T2, T3, T4, string>> expression);

        IDbQuery<T1, T2, T3, T4, TJoin> Join<TJoin>(Expression<Func<T1, T2, T3, T4, TJoin, bool>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IDbQuery<T1, T2, T3, T4, TJoin> Join<TJoin>(Expression<Func<T1, T2, T3, T4, TJoin, string>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IOrderedDbQuery<T1, T2, T3, T4> OrderBy<TField>(params Expression<Func<T1, T2, T3, T4, TField>>[] orderFields);

        IOrderedDbQuery<T1, T2, T3, T4> OrderByDescending<TField>(params Expression<Func<T1, T2, T3, T4, TField>>[] orderFields);

        IGroupingDbQuery<T1, T2, T3, T4> GroupBy<TFields>(Expression<Func<T1, T2, T3, T4, TFields>> fields);
    }

    public interface IGeneralDbQuery<T1, T2, T3, T4, T5> : IDbQuery, ISelectedDbQuery<T1>, IAggregateDbQuery<T1, T2, T3, T4, T5>
    {
        ISelectedDbQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> selector);

        IDbQuery<T1, T2, T3, T4, T5> Where(Expression<Func<T1, T2, T3, T4, T5, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5> Where(Expression<Func<T1, T2, T3, T4, T5, string>> expression);

        IOrderedDbQuery<T1, T2, T3, T4, T5> OrderBy<TField>(params Expression<Func<T1, T2, T3, T4, T5, TField>>[] orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5> OrderByDescending<TField>(params Expression<Func<T1, T2, T3, T4, T5, TField>>[] orderFields);

        IGroupingDbQuery<T1, T2, T3, T4, T5> GroupBy<TFields>(Expression<Func<T1, T2, T3, T4, T5, TFields>> fields);
    }
}
