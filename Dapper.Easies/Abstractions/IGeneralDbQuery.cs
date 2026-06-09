using System;
using System.Linq.Expressions;

namespace Dapper.Easies
{
    public interface IGeneralDbQuery<T> : IDbQuery, ISelectedDbQuery<T>, IAggregateDbQuery<T>
    {
        ISelectedDbQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> selector);

        ISelectedDbQuery<TResult> Select<TResult>();

        IDbQuery<T> Where(Expression<Func<T, bool>> predicate);

        IDbQuery<T> Where(Expression<Func<T, string>> expression);

        IDbQuery<T, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, JoinType type = JoinType.Inner) where TJoin : class;

        IDbQuery<T, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, Expression<Func<T, TJoin, bool>> on, JoinType type = JoinType.Inner) where TJoin : class;

        IDbQuery<T, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, Expression<Func<T, TJoin, string>> on, JoinType type = JoinType.Inner) where TJoin : class;

        IDbQuery<T, TJoin> Join<TJoin>(JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IDbQuery<T, TJoin> Join<TJoin>(Expression<Func<T, TJoin, bool>> on, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IDbQuery<T, TJoin> Join<TJoin>(Expression<Func<T, TJoin, string>> on, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IOrderedDbQuery<T> OrderBy(Expression<Func<T, object>> orderFields);

        IOrderedDbQuery<T> OrderByDescending(Expression<Func<T, object>> orderFields);

        IGroupingDbQuery<T> GroupBy(Expression<Func<T, object>> fields);
    }

    public interface IGeneralDbQuery<T1, T2> : IDbQuery, ISelectedDbQuery<T1>, IAggregateDbQuery<T1, T2>
    {
        ISelectedDbQuery<TResult> Select<TResult>(Expression<Func<T1, TResult>> selector);

        ISelectedDbQuery<TResult> Select<TResult>(Expression<Func<T1, T2, TResult>> selector);

        IDbQuery<T1, T2> Where(Expression<Func<T1, bool>> predicate);

        IDbQuery<T1, T2> Where(Expression<Func<T1, T2, bool>> predicate);

        IDbQuery<T1, T2> Where(Expression<Func<T1, string>> expression);

        IDbQuery<T1, T2> Where(Expression<Func<T1, T2, string>> expression);

        IDbQuery<T1, T2, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, JoinType type = JoinType.Inner) where TJoin : class;

        IDbQuery<T1, T2, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, Expression<Func<T1, T2, TJoin, bool>> on, JoinType type = JoinType.Inner) where TJoin : class;

        IDbQuery<T1, T2, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, Expression<Func<T1, T2, TJoin, string>> on, JoinType type = JoinType.Inner) where TJoin : class;

        IDbQuery<T1, T2, TJoin> Join<TJoin>(JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IDbQuery<T1, T2, TJoin> Join<TJoin>(Expression<Func<T1, T2, TJoin, bool>> on, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IDbQuery<T1, T2, TJoin> Join<TJoin>(Expression<Func<T1, T2, TJoin, string>> on, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IOrderedDbQuery<T1, T2> OrderBy(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2> OrderBy(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2> OrderByDescending(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2> OrderByDescending(Expression<Func<T1, object>> orderFields);

        IGroupingDbQuery<T1, T2> GroupBy(Expression<Func<T1, T2, object>> fields);

        IGroupingDbQuery<T1, T2> GroupBy(Expression<Func<T1, object>> fields);
    }

    public interface IGeneralDbQuery<T1, T2, T3> : IDbQuery, ISelectedDbQuery<T1>, IAggregateDbQuery<T1, T2, T3>
    {
        ISelectedDbQuery<TResult> Select<TResult>(Expression<Func<T1, TResult>> selector);

        ISelectedDbQuery<TResult> Select<TResult>(Expression<Func<T1, T2, TResult>> selector);

        ISelectedDbQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, TResult>> selector);

        IDbQuery<T1, T2, T3> Where(Expression<Func<T1, bool>> predicate);

        IDbQuery<T1, T2, T3> Where(Expression<Func<T1, T2, bool>> predicate);

        IDbQuery<T1, T2, T3> Where(Expression<Func<T1, T2, T3, bool>> predicate);

        IDbQuery<T1, T2, T3> Where(Expression<Func<T1, string>> expression);

        IDbQuery<T1, T2, T3> Where(Expression<Func<T1, T2, string>> expression);

        IDbQuery<T1, T2, T3> Where(Expression<Func<T1, T2, T3, string>> expression);

        IDbQuery<T1, T2, T3, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, JoinType type = JoinType.Inner) where TJoin : class;

        IDbQuery<T1, T2, T3, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, Expression<Func<T1, T2, T3, TJoin, bool>> on, JoinType type = JoinType.Inner) where TJoin : class;

        IDbQuery<T1, T2, T3, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, Expression<Func<T1, T2, T3, TJoin, string>> on, JoinType type = JoinType.Inner) where TJoin : class;

        IDbQuery<T1, T2, T3, TJoin> Join<TJoin>(JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IDbQuery<T1, T2, T3, TJoin> Join<TJoin>(Expression<Func<T1, T2, T3, TJoin, bool>> on, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IDbQuery<T1, T2, T3, TJoin> Join<TJoin>(Expression<Func<T1, T2, T3, TJoin, string>> on, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IOrderedDbQuery<T1, T2, T3> OrderBy(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3> OrderBy(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3> OrderBy(Expression<Func<T1, T2, T3, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3> OrderByDescending(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3> OrderByDescending(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3> OrderByDescending(Expression<Func<T1, T2, T3, object>> orderFields);

        IGroupingDbQuery<T1, T2, T3> GroupBy(Expression<Func<T1, object>> fields);

        IGroupingDbQuery<T1, T2, T3> GroupBy(Expression<Func<T1, T2, object>> fields);

        IGroupingDbQuery<T1, T2, T3> GroupBy(Expression<Func<T1, T2, T3, object>> fields);
    }

    public interface IGeneralDbQuery<T1, T2, T3, T4> : IDbQuery, ISelectedDbQuery<T1>, IAggregateDbQuery<T1, T2, T3, T4>
    {
        ISelectedDbQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> selector);

        IDbQuery<T1, T2, T3, T4> Where(Expression<Func<T1, bool>> predicate);

        IDbQuery<T1, T2, T3, T4> Where(Expression<Func<T1, T2, bool>> predicate);

        IDbQuery<T1, T2, T3, T4> Where(Expression<Func<T1, T2, T3, bool>> predicate);

        IDbQuery<T1, T2, T3, T4> Where(Expression<Func<T1, T2, T3, T4, bool>> predicate);

        IDbQuery<T1, T2, T3, T4> Where(Expression<Func<T1, string>> expression);

        IDbQuery<T1, T2, T3, T4> Where(Expression<Func<T1, T2, string>> expression);

        IDbQuery<T1, T2, T3, T4> Where(Expression<Func<T1, T2, T3, string>> expression);

        IDbQuery<T1, T2, T3, T4> Where(Expression<Func<T1, T2, T3, T4, string>> expression);

        IDbQuery<T1, T2, T3, T4, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, JoinType type = JoinType.Inner) where TJoin : class;

        IDbQuery<T1, T2, T3, T4, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, Expression<Func<T1, T2, T3, T4, TJoin, bool>> on, JoinType type = JoinType.Inner) where TJoin : class;

        IDbQuery<T1, T2, T3, T4, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, Expression<Func<T1, T2, T3, T4, TJoin, string>> on, JoinType type = JoinType.Inner) where TJoin : class;

        IDbQuery<T1, T2, T3, T4, TJoin> Join<TJoin>(JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IDbQuery<T1, T2, T3, T4, TJoin> Join<TJoin>(Expression<Func<T1, T2, T3, T4, TJoin, bool>> on, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IDbQuery<T1, T2, T3, T4, TJoin> Join<TJoin>(Expression<Func<T1, T2, T3, T4, TJoin, string>> on, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IOrderedDbQuery<T1, T2, T3, T4> OrderBy(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4> OrderBy(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4> OrderBy(Expression<Func<T1, T2, T3, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4> OrderBy(Expression<Func<T1, T2, T3, T4, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4> OrderByDescending(Expression<Func<T1, T2, T3, T4, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4> OrderByDescending(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4> OrderByDescending(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4> OrderByDescending(Expression<Func<T1, T2, T3, object>> orderFields);

        IGroupingDbQuery<T1, T2, T3, T4> GroupBy(Expression<Func<T1, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4> GroupBy(Expression<Func<T1, T2, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4> GroupBy(Expression<Func<T1, T2, T3, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4> GroupBy(Expression<Func<T1, T2, T3, T4, object>> fields);
    }

    public interface IGeneralDbQuery<T1, T2, T3, T4, T5> : IDbQuery, ISelectedDbQuery<T1>, IAggregateDbQuery<T1, T2, T3, T4, T5>
    {
        ISelectedDbQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> selector);

        IDbQuery<T1, T2, T3, T4, T5> Where(Expression<Func<T1, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5> Where(Expression<Func<T1, T2, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5> Where(Expression<Func<T1, T2, T3, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5> Where(Expression<Func<T1, T2, T3, T4, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5> Where(Expression<Func<T1, T2, T3, T4, T5, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5> Where(Expression<Func<T1, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5> Where(Expression<Func<T1, T2, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5> Where(Expression<Func<T1, T2, T3, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5> Where(Expression<Func<T1, T2, T3, T4, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5> Where(Expression<Func<T1, T2, T3, T4, T5, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, JoinType type = JoinType.Inner) where TJoin : class;

        IDbQuery<T1, T2, T3, T4, T5, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, Expression<Func<T1, T2, T3, T4, T5, TJoin, bool>> on, JoinType type = JoinType.Inner) where TJoin : class;

        IDbQuery<T1, T2, T3, T4, T5, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, Expression<Func<T1, T2, T3, T4, T5, TJoin, string>> on, JoinType type = JoinType.Inner) where TJoin : class;

        IDbQuery<T1, T2, T3, T4, T5, TJoin> Join<TJoin>(JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IDbQuery<T1, T2, T3, T4, T5, TJoin> Join<TJoin>(Expression<Func<T1, T2, T3, T4, T5, TJoin, bool>> on, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IDbQuery<T1, T2, T3, T4, T5, TJoin> Join<TJoin>(Expression<Func<T1, T2, T3, T4, T5, TJoin, string>> on, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IOrderedDbQuery<T1, T2, T3, T4, T5> OrderBy(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5> OrderBy(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5> OrderBy(Expression<Func<T1, T2, T3, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5> OrderBy(Expression<Func<T1, T2, T3, T4, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5> OrderBy(Expression<Func<T1, T2, T3, T4, T5, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5> OrderByDescending(Expression<Func<T1, T2, T3, T4, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5> OrderByDescending(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5> OrderByDescending(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5> OrderByDescending(Expression<Func<T1, T2, T3, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5> OrderByDescending(Expression<Func<T1, T2, T3, T4, T5, object>> orderFields);

        IGroupingDbQuery<T1, T2, T3, T4, T5> GroupBy(Expression<Func<T1, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5> GroupBy(Expression<Func<T1, T2, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5> GroupBy(Expression<Func<T1, T2, T3, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5> GroupBy(Expression<Func<T1, T2, T3, T4, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5> GroupBy(Expression<Func<T1, T2, T3, T4, T5, object>> fields);
    }

    public interface IGeneralDbQuery<T1, T2, T3, T4, T5, T6> : IDbQuery, ISelectedDbQuery<T1>, IAggregateDbQuery<T1, T2, T3, T4, T5, T6>
    {
        ISelectedDbQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> selector);

        IDbQuery<T1, T2, T3, T4, T5, T6> Where(Expression<Func<T1, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6> Where(Expression<Func<T1, T2, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6> Where(Expression<Func<T1, T2, T3, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6> Where(Expression<Func<T1, T2, T3, T4, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6> Where(Expression<Func<T1, T2, T3, T4, T5, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6> Where(Expression<Func<T1, T2, T3, T4, T5, T6, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6> Where(Expression<Func<T1, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6> Where(Expression<Func<T1, T2, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6> Where(Expression<Func<T1, T2, T3, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6> Where(Expression<Func<T1, T2, T3, T4, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6> Where(Expression<Func<T1, T2, T3, T4, T5, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6> Where(Expression<Func<T1, T2, T3, T4, T5, T6, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, JoinType type = JoinType.Inner) where TJoin : class;

        IDbQuery<T1, T2, T3, T4, T5, T6, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, Expression<Func<T1, T2, T3, T4, T5, T6, TJoin, bool>> on, JoinType type = JoinType.Inner) where TJoin : class;

        IDbQuery<T1, T2, T3, T4, T5, T6, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, Expression<Func<T1, T2, T3, T4, T5, T6, TJoin, string>> on, JoinType type = JoinType.Inner) where TJoin : class;

        IDbQuery<T1, T2, T3, T4, T5, T6, TJoin> Join<TJoin>(JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IDbQuery<T1, T2, T3, T4, T5, T6, TJoin> Join<TJoin>(Expression<Func<T1, T2, T3, T4, T5, T6, TJoin, bool>> on, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IDbQuery<T1, T2, T3, T4, T5, T6, TJoin> Join<TJoin>(Expression<Func<T1, T2, T3, T4, T5, T6, TJoin, string>> on, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T1, T2, T3, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T1, T2, T3, T4, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T1, T2, T3, T4, T5, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6> OrderBy(Expression<Func<T1, T2, T3, T4, T5, T6, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6> OrderByDescending(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6> OrderByDescending(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6> OrderByDescending(Expression<Func<T1, T2, T3, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6> OrderByDescending(Expression<Func<T1, T2, T3, T4, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6> OrderByDescending(Expression<Func<T1, T2, T3, T4, T5, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6> OrderByDescending(Expression<Func<T1, T2, T3, T4, T5, T6, object>> orderFields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T1, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T1, T2, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T1, T2, T3, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T1, T2, T3, T4, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T1, T2, T3, T4, T5, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6> GroupBy(Expression<Func<T1, T2, T3, T4, T5, T6, object>> fields);
    }

    public interface IGeneralDbQuery<T1, T2, T3, T4, T5, T6, T7> : IDbQuery, ISelectedDbQuery<T1>, IAggregateDbQuery<T1, T2, T3, T4, T5, T6, T7>
    {
        ISelectedDbQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> selector);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T1, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T1, T2, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T1, T2, T3, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T1, T2, T3, T4, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T1, T2, T3, T4, T5, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T1, T2, T3, T4, T5, T6, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T1, T2, T3, T4, T5, T6, T7, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T1, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T1, T2, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T1, T2, T3, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T1, T2, T3, T4, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T1, T2, T3, T4, T5, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T1, T2, T3, T4, T5, T6, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7> Where(Expression<Func<T1, T2, T3, T4, T5, T6, T7, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, JoinType type = JoinType.Inner) where TJoin : class;

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, Expression<Func<T1, T2, T3, T4, T5, T6, T7, TJoin, bool>> on, JoinType type = JoinType.Inner) where TJoin : class;

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, Expression<Func<T1, T2, T3, T4, T5, T6, T7, TJoin, string>> on, JoinType type = JoinType.Inner) where TJoin : class;

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, TJoin> Join<TJoin>(JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, TJoin> Join<TJoin>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, TJoin, bool>> on, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, TJoin> Join<TJoin>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, TJoin, string>> on, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T1, T2, T3, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T1, T2, T3, T4, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T1, T2, T3, T4, T5, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T1, T2, T3, T4, T5, T6, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> OrderBy(Expression<Func<T1, T2, T3, T4, T5, T6, T7, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> OrderByDescending(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> OrderByDescending(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> OrderByDescending(Expression<Func<T1, T2, T3, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> OrderByDescending(Expression<Func<T1, T2, T3, T4, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> OrderByDescending(Expression<Func<T1, T2, T3, T4, T5, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> OrderByDescending(Expression<Func<T1, T2, T3, T4, T5, T6, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> OrderByDescending(Expression<Func<T1, T2, T3, T4, T5, T6, T7, object>> orderFields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T1, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T1, T2, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T1, T2, T3, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T1, T2, T3, T4, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T1, T2, T3, T4, T5, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T1, T2, T3, T4, T5, T6, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6, T7> GroupBy(Expression<Func<T1, T2, T3, T4, T5, T6, T7, object>> fields);
    }

    public interface IGeneralDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> : IDbQuery, ISelectedDbQuery<T1>, IAggregateDbQuery<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        ISelectedDbQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> selector);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T1, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T1, T2, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T1, T2, T3, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T1, T2, T3, T4, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T1, T2, T3, T4, T5, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T1, T2, T3, T4, T5, T6, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T1, T2, T3, T4, T5, T6, T7, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T1, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T1, T2, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T1, T2, T3, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T1, T2, T3, T4, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T1, T2, T3, T4, T5, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T1, T2, T3, T4, T5, T6, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T1, T2, T3, T4, T5, T6, T7, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> Where(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, JoinType type = JoinType.Inner) where TJoin : class;

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TJoin, bool>> on, JoinType type = JoinType.Inner) where TJoin : class;

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TJoin, string>> on, JoinType type = JoinType.Inner) where TJoin : class;

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, TJoin> Join<TJoin>(JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, TJoin> Join<TJoin>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TJoin, bool>> on, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, TJoin> Join<TJoin>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TJoin, string>> on, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T1, T2, T3, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T1, T2, T3, T4, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T1, T2, T3, T4, T5, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T1, T2, T3, T4, T5, T6, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T1, T2, T3, T4, T5, T6, T7, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> OrderBy(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> OrderByDescending(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> OrderByDescending(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> OrderByDescending(Expression<Func<T1, T2, T3, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> OrderByDescending(Expression<Func<T1, T2, T3, T4, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> OrderByDescending(Expression<Func<T1, T2, T3, T4, T5, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> OrderByDescending(Expression<Func<T1, T2, T3, T4, T5, T6, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> OrderByDescending(Expression<Func<T1, T2, T3, T4, T5, T6, T7, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> OrderByDescending(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, object>> orderFields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T1, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T1, T2, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T1, T2, T3, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T1, T2, T3, T4, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T1, T2, T3, T4, T5, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T1, T2, T3, T4, T5, T6, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T1, T2, T3, T4, T5, T6, T7, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> GroupBy(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, object>> fields);
    }

    public interface IGeneralDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> : IDbQuery, ISelectedDbQuery<T1>, IAggregateDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        ISelectedDbQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> selector);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T1, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T1, T2, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T1, T2, T3, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T1, T2, T3, T4, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T1, T2, T3, T4, T5, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T1, T2, T3, T4, T5, T6, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T1, T2, T3, T4, T5, T6, T7, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, bool>> predicate);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T1, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T1, T2, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T1, T2, T3, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T1, T2, T3, T4, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T1, T2, T3, T4, T5, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T1, T2, T3, T4, T5, T6, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T1, T2, T3, T4, T5, T6, T7, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, string>> expression);

        IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> Where(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, string>> expression);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T1, T2, T3, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T1, T2, T3, T4, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T1, T2, T3, T4, T5, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T1, T2, T3, T4, T5, T6, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T1, T2, T3, T4, T5, T6, T7, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> OrderBy(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> OrderByDescending(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> OrderByDescending(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> OrderByDescending(Expression<Func<T1, T2, T3, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> OrderByDescending(Expression<Func<T1, T2, T3, T4, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> OrderByDescending(Expression<Func<T1, T2, T3, T4, T5, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> OrderByDescending(Expression<Func<T1, T2, T3, T4, T5, T6, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> OrderByDescending(Expression<Func<T1, T2, T3, T4, T5, T6, T7, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> OrderByDescending(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> OrderByDescending(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, object>> orderFields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T1, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T1, T2, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T1, T2, T3, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T1, T2, T3, T4, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T1, T2, T3, T4, T5, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T1, T2, T3, T4, T5, T6, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T1, T2, T3, T4, T5, T6, T7, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, object>> fields);

        IGroupingDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> GroupBy(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, object>> fields);
    }
}
