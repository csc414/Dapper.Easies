using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Easies
{
    public interface IAggregateDbQuery<T> : IDbQuery
    {
        Task<long> CountAsync();

        Task<long> CountAsync(Expression<Func<T, object>> field);

        Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> field);

        Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> field);

        Task<decimal> AvgAsync<TField>(Expression<Func<T, TField>> field);

        Task<decimal> SumAsync<TField>(Expression<Func<T, TField>> field);
    }

    public interface IAggregateDbQuery<T1, T2> : IAggregateDbQuery<T1>
    {
        Task<long> CountAsync(Expression<Func<T1, T2, object>> field);

        Task<TResult> MaxAsync<TResult>(Expression<Func<T1, T2, TResult>> field);

        Task<TResult> MinAsync<TResult>(Expression<Func<T1, T2, TResult>> field);

        Task<decimal> AvgAsync<TField>(Expression<Func<T1, T2, TField>> field);

        Task<decimal> SumAsync<TField>(Expression<Func<T1, T2, TField>> field);
    }

    public interface IAggregateDbQuery<T1, T2, T3> : IAggregateDbQuery<T1, T2>
    {
        Task<long> CountAsync(Expression<Func<T1, T2, T3, object>> field);

        Task<TResult> MaxAsync<TResult>(Expression<Func<T1, T2, T3, TResult>> field);

        Task<TResult> MinAsync<TResult>(Expression<Func<T1, T2, T3, TResult>> field);

        Task<decimal> AvgAsync<TField>(Expression<Func<T1, T2, T3, TField>> field);

        Task<decimal> SumAsync<TField>(Expression<Func<T1, T2, T3, TField>> field);
    }

    public interface IAggregateDbQuery<T1, T2, T3, T4> : IAggregateDbQuery<T1, T2, T3>
    {
        Task<long> CountAsync(Expression<Func<T1, T2, T3, T4, object>> field);

        Task<TResult> MaxAsync<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> field);

        Task<TResult> MinAsync<TResult>(Expression<Func<T1, T2, T3, T4, TResult>> field);

        Task<decimal> AvgAsync<TField>(Expression<Func<T1, T2, T3, T4, TField>> field);

        Task<decimal> SumAsync<TField>(Expression<Func<T1, T2, T3, T4, TField>> field);
    }

    public interface IAggregateDbQuery<T1, T2, T3, T4, T5> : IAggregateDbQuery<T1, T2, T3, T4>
    {
        Task<long> CountAsync(Expression<Func<T1, T2, T3, T4, T5, object>> field);

        Task<TResult> MaxAsync<TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> field);

        Task<TResult> MinAsync<TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> field);

        Task<decimal> AvgAsync<TField>(Expression<Func<T1, T2, T3, T4, T5, TField>> field);

        Task<decimal> SumAsync<TField>(Expression<Func<T1, T2, T3, T4, T5, TField>> field);
    }
}
