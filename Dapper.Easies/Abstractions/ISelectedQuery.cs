using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Dapper.Easies
{
    public interface ISelectedDbQuery<T> : IDbQuery
    {
        Task<T> FirstAsync();

        Task<T> FirstOrDefaultAsync();

        Task<IEnumerable<T>> QueryAsync();

        Task<IEnumerable<TResult>> QueryAsync<TResult>() where TResult : ITuple;

        Task<(IEnumerable<T> data, long total, int max_page)> GetPagerAsync(int page, int size);

        Task<(IEnumerable<TResult> data, long total, int max_page)> GetPagerAsync<TResult>(int page, int size) where TResult : ITuple;

        Task<(IEnumerable<T> data, long total)> GetLimitAsync(int skip, int take);

        Task<(IEnumerable<TResult> data, long total)> GetLimitAsync<TResult>(int skip, int take) where TResult : ITuple;

        Task<bool> ExistAsync();
    }
}
