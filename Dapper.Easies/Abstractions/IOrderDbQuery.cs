using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
    public interface IOrderedDbQuery<T> : IDbQuery<T>
    {
        IOrderedDbQuery<T> ThenBy(params Expression<Func<T, object>>[] orderFields);

        IOrderedDbQuery<T> ThenByDescending(params Expression<Func<T, object>>[] orderFields);
    }

    public interface IOrderedDbQuery<T1, T2> : IDbQuery<T1, T2>
    {
        IOrderedDbQuery<T1, T2> ThenBy(params Expression<Func<T1, T2, object>>[] orderFields);

        IOrderedDbQuery<T1, T2> ThenByDescending(params Expression<Func<T1, T2, object>>[] orderFields);
    }
}
