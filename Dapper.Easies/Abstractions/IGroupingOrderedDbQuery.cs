using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
    public interface IGroupingOrderedDbQuery<T> : IGroupingSelectedDbQuery<T>
    {
        IGroupingOrderedDbQuery<T> ThenBy<TField>(params Expression<Func<T, TField>>[] orderFields);

        IGroupingOrderedDbQuery<T> ThenByDescending<TField>(params Expression<Func<T, TField>>[] orderFields);
    }

    public interface IGroupingOrderedDbQuery<T1, T2> : IGroupingSelectedDbQuery<T1>
    {
        IGroupingOrderedDbQuery<T1, T2> ThenBy<TField>(params Expression<Func<T1, T2, TField>>[] orderFields);

        IGroupingOrderedDbQuery<T1, T2> ThenByDescending<TField>(params Expression<Func<T1, T2, TField>>[] orderFields);
    }

    public interface IGroupingOrderedDbQuery<T1, T2, T3> : IGroupingSelectedDbQuery<T1>
    {
        IGroupingOrderedDbQuery<T1, T2, T3> ThenBy<TField>(params Expression<Func<T1, T2, T3, TField>>[] orderFields);

        IGroupingOrderedDbQuery<T1, T2, T3> ThenByDescending<TField>(params Expression<Func<T1, T2, T3, TField>>[] orderFields);
    }

    public interface IGroupingOrderedDbQuery<T1, T2, T3, T4> : IGroupingSelectedDbQuery<T1>
    {
        IGroupingOrderedDbQuery<T1, T2, T3, T4> ThenBy<TField>(params Expression<Func<T1, T2, T3, T4, TField>>[] orderFields);

        IGroupingOrderedDbQuery<T1, T2, T3, T4> ThenByDescending<TField>(params Expression<Func<T1, T2, T3, T4, TField>>[] orderFields);
    }

    public interface IGroupingOrderedDbQuery<T1, T2, T3, T4, T5> : IGroupingSelectedDbQuery<T1>
    {
        IGroupingOrderedDbQuery<T1, T2, T3, T4, T5> ThenBy<TField>(params Expression<Func<T1, T2, T3, T4, T5, TField>>[] orderFields);

        IGroupingOrderedDbQuery<T1, T2, T3, T4, T5> ThenByDescending<TField>(params Expression<Func<T1, T2, T3, T4, T5, TField>>[] orderFields);
    }
}
