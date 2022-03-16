using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
    public interface IOrderedDbQuery<T> : IGeneralDbQuery<T>
    {
        IOrderedDbQuery<T> ThenBy<TField>(params Expression<Func<T, TField>>[] orderFields);

        IOrderedDbQuery<T> ThenByDescending<TField>(params Expression<Func<T, TField>>[] orderFields);
    }

    public interface IOrderedDbQuery<T1, T2> : IDbQuery<T1, T2>
    {
        IOrderedDbQuery<T1, T2> ThenBy<TField>(params Expression<Func<T1, T2, TField>>[] orderFields);

        IOrderedDbQuery<T1, T2> ThenByDescending<TField>(params Expression<Func<T1, T2, TField>>[] orderFields);
    }

    public interface IOrderedDbQuery<T1, T2, T3> : IDbQuery<T1, T2, T3>
    {
        IOrderedDbQuery<T1, T2, T3> ThenBy<TField>(params Expression<Func<T1, T2, T3, TField>>[] orderFields);

        IOrderedDbQuery<T1, T2, T3> ThenByDescending<TField>(params Expression<Func<T1, T2, T3, TField>>[] orderFields);
    }

    public interface IOrderedDbQuery<T1, T2, T3, T4> : IDbQuery<T1, T2, T3, T4>
    {
        IOrderedDbQuery<T1, T2, T3, T4> ThenBy<TField>(params Expression<Func<T1, T2, T3, T4, TField>>[] orderFields);

        IOrderedDbQuery<T1, T2, T3, T4> ThenByDescending<TField>(params Expression<Func<T1, T2, T3, T4, TField>>[] orderFields);
    }

    public interface IOrderedDbQuery<T1, T2, T3, T4, T5> : IDbQuery<T1, T2, T3, T4, T5>
    {
        IOrderedDbQuery<T1, T2, T3, T4, T5> ThenBy<TField>(params Expression<Func<T1, T2, T3, T4, T5, TField>>[] orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5> ThenByDescending<TField>(params Expression<Func<T1, T2, T3, T4, T5, TField>>[] orderFields);
    }
}
