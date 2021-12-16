using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Easies
{
    public interface ISelectedQuery<T> : IDbQuery
    {
        Task<T> FirstAsync();

        Task<T> FirstOrDefaultAsync();
    }
}
