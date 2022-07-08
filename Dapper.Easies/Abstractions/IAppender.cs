using System;
using System.Linq.Expressions;

namespace Dapper.Easies
{
    public interface IAppender<T>
    {
        Expression<Func<T, bool>> Append();
    }
}
