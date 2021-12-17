using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Easies
{
    public interface IAggregateQuery<T> : IDbQuery
    {
        Task<int> CountAsync(Expression<Func<T, object>> field = null);

        Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> field);

        Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> field);
    }

    public interface IAggregateQuery<T1, T2> : IDbQuery
    {
        Task<int> CountAsync(Expression<Func<T1, T2, object>> field = null);

        Task<TResult> MaxAsync<TResult>(Expression<Func<T1, T2, TResult>> field);

        Task<TResult> MinAsync<TResult>(Expression<Func<T1, T2, TResult>> field);
    }

    public interface IAggregateQuery<T1, T2, T3> : IDbQuery
    {
        Task<int> CountAsync(Expression<Func<T1, T2, T3, object>> field = null);

        Task<TResult> MaxAsync<TResult>(Expression<Func<T1, T2, T3, TResult>> field);

        Task<TResult> MinAsync<TResult>(Expression<Func<T1, T2, T3, TResult>> field);
    }

    public interface IAggregateQuery<T1, T2, T3, T4> : IDbQuery
    {
        Task<int> CountAsync(Expression<Func<T1, T2, T3, T4, object>> field = null);

        Task<TResult> MaxAsync<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> field);

        Task<TResult> MinAsync<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> field);
    }

    public interface IAggregateQuery<T1, T2, T3, T4, T5> : IDbQuery
    {
        Task<int> CountAsync(Expression<Func<T1, T2, T3, T4, T5, object>> field = null);

        Task<TResult> MaxAsync<TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> field);

        Task<TResult> MinAsync<TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> field);
    }
}
