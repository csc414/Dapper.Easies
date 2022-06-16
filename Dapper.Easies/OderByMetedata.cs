using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
    public class OrderByMetedata
    {
        public OrderByMetedata(Expression expression, SortType sortType)
        {
            Expression = expression;
            SortType = sortType;
        }

        public Expression Expression { get; }

        public SortType SortType { get; }
    }
}
