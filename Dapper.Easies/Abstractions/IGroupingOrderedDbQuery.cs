using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
    public interface IGroupingOrderedDbQuery<T> : IGroupingSelectedDbQuery<T>
    {
        IGroupingOrderedDbQuery<T> ThenBy(params Expression<Func<T, object>>[] orderFields);

        IGroupingOrderedDbQuery<T> ThenByDescending(params Expression<Func<T, object>>[] orderFields);
    }

    public interface IGroupingOrderedDbQuery<T1, T2> : IGroupingSelectedDbQuery<T1>
    {
        IGroupingOrderedDbQuery<T1, T2> ThenBy(params Expression<Func<T1, T2, object>>[] orderFields);

        IGroupingOrderedDbQuery<T1, T2> ThenByDescending(params Expression<Func<T1, T2, object>>[] orderFields);
    }

    public interface IGroupingOrderedDbQuery<T1, T2, T3> : IGroupingSelectedDbQuery<T1>
    {
        IGroupingOrderedDbQuery<T1, T2, T3> ThenBy(params Expression<Func<T1, T2, T3, object>>[] orderFields);

        IGroupingOrderedDbQuery<T1, T2, T3> ThenByDescending(params Expression<Func<T1, T2, T3, object>>[] orderFields);
    }

    public interface IGroupingOrderedDbQuery<T1, T2, T3, T4> : IGroupingSelectedDbQuery<T1>
    {
        IGroupingOrderedDbQuery<T1, T2, T3, T4> ThenBy(params Expression<Func<T1, T2, T3, T4, object>>[] orderFields);

        IGroupingOrderedDbQuery<T1, T2, T3, T4> ThenByDescending(params Expression<Func<T1, T2, T3, T4, object>>[] orderFields);
    }

    public interface IGroupingOrderedDbQuery<T1, T2, T3, T4, T5> : IGroupingSelectedDbQuery<T1>
    {
        IGroupingOrderedDbQuery<T1, T2, T3, T4, T5> ThenBy(params Expression<Func<T1, T2, T3, T4, T5, object>>[] orderFields);

        IGroupingOrderedDbQuery<T1, T2, T3, T4, T5> ThenByDescending(params Expression<Func<T1, T2, T3, T4, T5, object>>[] orderFields);
    }
}
