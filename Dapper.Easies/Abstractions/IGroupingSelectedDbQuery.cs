using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
    public interface IGroupingSelectedDbQuery<T> : ISelectedDbQuery<T>
    {
        IGroupingOrderedDbQuery<T> OrderBy<TField>(params Expression<Func<T, TField>>[] orderFields);

        IGroupingOrderedDbQuery<T> OrderByDescending<TField>(params Expression<Func<T, TField>>[] orderFields);
    }

    public interface IGroupingSelectedDbQuery<T1, T2> : ISelectedDbQuery<T1>
    {
        IGroupingOrderedDbQuery<T1, T2> OrderBy<TField>(params Expression<Func<T1, T2, TField>>[] orderFields);

        IGroupingOrderedDbQuery<T1, T2> OrderByDescending<TField>(params Expression<Func<T1, T2, TField>>[] orderFields);
    }

    public interface IGroupingSelectedDbQuery<T1, T2, T3> : ISelectedDbQuery<T1>
    {
        IGroupingOrderedDbQuery<T1, T2, T3> OrderBy<TField>(params Expression<Func<T1, T2, T3, TField>>[] orderFields);

        IGroupingOrderedDbQuery<T1, T2, T3> OrderByDescending<TField>(params Expression<Func<T1, T2, T3, TField>>[] orderFields);
    }

    public interface IGroupingSelectedDbQuery<T1, T2, T3, T4> : ISelectedDbQuery<T1>
    {
        IGroupingOrderedDbQuery<T1, T2, T3, T4> OrderBy<TField>(params Expression<Func<T1, T2, T3, T4, TField>>[] orderFields);

        IGroupingOrderedDbQuery<T1, T2, T3, T4> OrderByDescending<TField>(params Expression<Func<T1, T2, T3, T4, TField>>[] orderFields);
    }

    public interface IGroupingSelectedDbQuery<T1, T2, T3, T4, T5> : ISelectedDbQuery<T1>
    {
        IGroupingOrderedDbQuery<T1, T2, T3, T4, T5> OrderBy<TField>(params Expression<Func<T1, T2, T3, T4, T5, TField>>[] orderFields);

        IGroupingOrderedDbQuery<T1, T2, T3, T4, T5> OrderByDescending<TField>(params Expression<Func<T1, T2, T3, T4, T5, TField>>[] orderFields);
    }
}
