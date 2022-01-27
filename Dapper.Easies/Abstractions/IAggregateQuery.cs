using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Easies
{
    public interface IAggregateDbQuery<T> : IDbQuery
    {
        Task<long> CountAsync(Expression<Func<T, object>> field = null);

        Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> field);

        Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> field);

        Task<long> AvgAsync<TField>(Expression<Func<T, TField>> field);

        Task<long> SumAsync<TField>(Expression<Func<T, TField>> field);
    }

    public interface IAggregateDbQuery<T1, T2> : IDbQuery
    {
        Task<long> CountAsync(Expression<Func<T1, T2, object>> field = null);

        Task<TResult> MaxAsync<TResult>(Expression<Func<T1, T2, TResult>> field);

        Task<TResult> MinAsync<TResult>(Expression<Func<T1, T2, TResult>> field);

        Task<long> AvgAsync<TField>(Expression<Func<T1, T2, TField>> field);

        Task<long> SumAsync<TField>(Expression<Func<T1, T2, TField>> field);
    }

    public interface IAggregateDbQuery<T1, T2, T3> : IDbQuery
    {
        Task<long> CountAsync(Expression<Func<T1, T2, T3, object>> field = null);

        Task<TResult> MaxAsync<TResult>(Expression<Func<T1, T2, T3, TResult>> field);

        Task<TResult> MinAsync<TResult>(Expression<Func<T1, T2, T3, TResult>> field);

        Task<long> AvgAsync<TField>(Expression<Func<T1, T2, T3, TField>> field);

        Task<long> SumAsync<TField>(Expression<Func<T1, T2, T3, TField>> field);
    }

    public interface IAggregateDbQuery<T1, T2, T3, T4> : IDbQuery
    {
        Task<long> CountAsync(Expression<Func<T1, T2, T3, T4, object>> field = null);

        Task<TResult> MaxAsync<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> field);

        Task<TResult> MinAsync<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> field);

        Task<long> AvgAsync<TField>(Expression<Func<T1, T2, T3, T4, TField>> field);

        Task<long> SumAsync<TField>(Expression<Func<T1, T2, T3, T4, TField>> field);
    }

    public interface IAggregateDbQuery<T1, T2, T3, T4, T5> : IDbQuery
    {
        Task<long> CountAsync(Expression<Func<T1, T2, T3, T4, T5, object>> field = null);

        Task<TResult> MaxAsync<TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> field);

        Task<TResult> MinAsync<TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> field);

        Task<long> AvgAsync<TField>(Expression<Func<T1, T2, T3, T4, T5, TField>> field);

        Task<long> SumAsync<TField>(Expression<Func<T1, T2, T3, T4, T5, TField>> field);
    }
}
