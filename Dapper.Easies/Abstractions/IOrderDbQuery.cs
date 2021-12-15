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
}
