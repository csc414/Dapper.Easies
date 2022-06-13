using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
    public interface IGroupingOrderedDbQuery<T> : IGroupingSelectedDbQuery<T>
    {
        IGroupingOrderedDbQuery<T> ThenBy(Expression<Func<T, object>> orderFields);

        IGroupingOrderedDbQuery<T> ThenByDescending(Expression<Func<T, object>> orderFields);
    }
}
