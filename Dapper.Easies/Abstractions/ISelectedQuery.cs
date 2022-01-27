using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Easies
{
    public interface ISelectedDbQuery<T> : IDbQuery
    {
        Task<T> FirstAsync();

        Task<T> FirstOrDefaultAsync();

        Task<IEnumerable<T>> QueryAsync();
    }
}
