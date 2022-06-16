using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
    public interface IGroupingSelectedDbQuery<T> : ISelectedDbQuery<T>
    {
        IGroupingOrderedDbQuery<T> OrderBy(Expression<Func<T, object>> orderFields);

        IGroupingOrderedDbQuery<T> OrderByDescending(Expression<Func<T, object>> orderFields);
    }
}
