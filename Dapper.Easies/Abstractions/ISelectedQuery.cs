using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Easies
{
    public interface ISelectedQuery<T>
    {
        Task<T> FirstAsync();

        Task<T> FirstOrDefaultAsync();
    }
}
