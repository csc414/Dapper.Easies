using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
    public interface IGroupingSelectedDbQuery<T> : ISelectedDbQuery<T>
    {
        IGroupingOrderedDbQuery<T> OrderBy(params Expression<Func<T, object>>[] orderFields);

        IGroupingOrderedDbQuery<T> OrderByDescending(params Expression<Func<T, object>>[] orderFields);
    }

    public interface IGroupingSelectedDbQuery<T1, T2> : ISelectedDbQuery<T1>
    {
        IGroupingOrderedDbQuery<T1, T2> OrderBy(params Expression<Func<T1, T2, object>>[] orderFields);

        IGroupingOrderedDbQuery<T1, T2> OrderByDescending(params Expression<Func<T1, T2, object>>[] orderFields);
    }

    public interface IGroupingSelectedDbQuery<T1, T2, T3> : ISelectedDbQuery<T1>
    {
        IGroupingOrderedDbQuery<T1, T2, T3> OrderBy(params Expression<Func<T1, T2, T3, object>>[] orderFields);

        IGroupingOrderedDbQuery<T1, T2, T3> OrderByDescending(params Expression<Func<T1, T2, T3, object>>[] orderFields);
    }

    public interface IGroupingSelectedDbQuery<T1, T2, T3, T4> : ISelectedDbQuery<T1>
    {
        IGroupingOrderedDbQuery<T1, T2, T3, T4> OrderBy(params Expression<Func<T1, T2, T3, T4, object>>[] orderFields);

        IGroupingOrderedDbQuery<T1, T2, T3, T4> OrderByDescending(params Expression<Func<T1, T2, T3, T4, object>>[] orderFields);
    }

    public interface IGroupingSelectedDbQuery<T1, T2, T3, T4, T5> : ISelectedDbQuery<T1>
    {
        IGroupingOrderedDbQuery<T1, T2, T3, T4, T5> OrderBy(params Expression<Func<T1, T2, T3, T4, T5, object>>[] orderFields);

        IGroupingOrderedDbQuery<T1, T2, T3, T4, T5> OrderByDescending(params Expression<Func<T1, T2, T3, T4, T5, object>>[] orderFields);
    }
}
