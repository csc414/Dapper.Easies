using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Easies
{
    public interface IOperateDbQuery<T>
    {
        Task<int> DeleteAsync();

        Task<int> UpdateAsync(Expression<Func<T>> updateFields);

        Task<int> UpdateAsync(Expression<Func<T, T>> updateFields);
    }
}
