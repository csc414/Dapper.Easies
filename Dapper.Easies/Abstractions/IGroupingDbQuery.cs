using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
    public interface IGroupingDbQuery<T>
    {
        IGroupingSelectedDbQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> selector);

        IGroupingDbQuery<T> Having(Expression<Func<T, string>> expression);

        IGroupingDbQuery<T> Having(Expression<Func<T, bool>> predicate);
    }

    public interface IGroupingDbQuery<T1, T2>
    {
        IGroupingSelectedDbQuery<TResult> Select<TResult>(Expression<Func<T1, T2, TResult>> selector);

        IGroupingDbQuery<T1, T2> Having(Expression<Func<T1, T2, string>> expression);

        IGroupingDbQuery<T1, T2> Having(Expression<Func<T1, T2, bool>> predicate);
    }

    public interface IGroupingDbQuery<T1, T2, T3>
    {
        IGroupingSelectedDbQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, TResult>> selector);

        IGroupingDbQuery<T1, T2, T3> Having(Expression<Func<T1, T2, T3, string>> expression);

        IGroupingDbQuery<T1, T2, T3> Having(Expression<Func<T1, T2, T3, bool>> predicate);
    }

    public interface IGroupingDbQuery<T1, T2, T3, T4>
    {
        IGroupingSelectedDbQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> selector);

        IGroupingDbQuery<T1, T2, T3, T4> Having(Expression<Func<T1, T2, T3, T4, string>> expression);

        IGroupingDbQuery<T1, T2, T3, T4> Having(Expression<Func<T1, T2, T3, T4, bool>> predicate);
    }

    public interface IGroupingDbQuery<T1, T2, T3, T4, T5>
    {
        IGroupingSelectedDbQuery<TResult> Select<TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> selector);

        IGroupingDbQuery<T1, T2, T3, T4, T5> Having(Expression<Func<T1, T2, T3, T4, T5, string>> expression);

        IGroupingDbQuery<T1, T2, T3, T4, T5> Having(Expression<Func<T1, T2, T3, T4, T5, bool>> predicate);
    }
}
