﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Dapper.Easies
{
    public interface ISelectedDbQuery<T> : IDbQuery
    {
        Task<T> FirstAsync();

        Task<T> FirstOrDefaultAsync();

        Task<TResult> FirstAsync<TResult>() where TResult : struct;

        Task<TResult?> FirstOrDefaultAsync<TResult>() where TResult : struct;

        Task<IEnumerable<T>> QueryAsync();

        Task<IEnumerable<TResult>> QueryAsync<TResult>() where TResult : struct;

        Task<(IEnumerable<T> data, long total, int max_page)> GetPagerAsync(int page, int size);

        Task<(IEnumerable<TResult> data, long total, int max_page)> GetPagerAsync<TResult>(int page, int size) where TResult : struct;

        Task<(IEnumerable<T> data, long total)> GetLimitAsync(int skip, int take);

        Task<(IEnumerable<TResult> data, long total)> GetLimitAsync<TResult>(int skip, int take) where TResult : struct;

        Task<bool> ExistAsync();
    }
}
