using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
    public interface IOrderedDbQuery<T> : IGeneralDbQuery<T>
    {
        IOrderedDbQuery<T> ThenBy(params Expression<Func<T, object>>[] orderFields);

        IOrderedDbQuery<T> ThenByDescending(params Expression<Func<T, object>>[] orderFields);
    }

    public interface IOrderedDbQuery<T1, T2> : IDbQuery<T1, T2>
    {
        IOrderedDbQuery<T1, T2> ThenBy(params Expression<Func<T1, T2, object>>[] orderFields);

        IOrderedDbQuery<T1, T2> ThenByDescending(params Expression<Func<T1, T2, object>>[] orderFields);
    }

    public interface IOrderedDbQuery<T1, T2, T3> : IDbQuery<T1, T2, T3>
    {
        IOrderedDbQuery<T1, T2, T3> ThenBy(params Expression<Func<T1, T2, T3, object>>[] orderFields);

        IOrderedDbQuery<T1, T2, T3> ThenByDescending(params Expression<Func<T1, T2, T3, object>>[] orderFields);
    }

    public interface IOrderedDbQuery<T1, T2, T3, T4> : IDbQuery<T1, T2, T3, T4>
    {
        IOrderedDbQuery<T1, T2, T3, T4> ThenBy(params Expression<Func<T1, T2, T3, T4, object>>[] orderFields);

        IOrderedDbQuery<T1, T2, T3, T4> ThenByDescending(params Expression<Func<T1, T2, T3, T4, object>>[] orderFields);
    }

    public interface IOrderedDbQuery<T1, T2, T3, T4, T5> : IDbQuery<T1, T2, T3, T4, T5>
    {
        IOrderedDbQuery<T1, T2, T3, T4, T5> ThenBy(params Expression<Func<T1, T2, T3, T4, T5, object>>[] orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5> ThenByDescending(params Expression<Func<T1, T2, T3, T4, T5, object>>[] orderFields);
    }
}
