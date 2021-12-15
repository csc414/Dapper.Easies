using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Easies
{
    public interface IDbQuery<T> : ISelectedQuery<T>
    {
        ISelectedQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> selector);

        IDbQuery<T, TJoin> Join<TJoin>(Expression<Predicate<T, TJoin>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject;

        public IOrderedDbQuery<T> OrderBy(params Expression<Func<T, object>>[] orderField);

        public IOrderedDbQuery<T> OrderByDescending(params Expression<Func<T, object>>[] orderField);
    }

    public interface IDbQuery<T1, T2> : IDbQuery<T1>
    {
        DbQuery<T1, T2, TJoin> Join<TJoin>(Expression<Predicate<T1, T2, TJoin>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject;
    }
}
