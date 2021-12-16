using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
    public class OrderByMetedata
    {
        public OrderByMetedata(IEnumerable<Expression> expressions, SortType sortType)
        {
            Expressions = expressions;
            SortType = sortType;
        }

        public IEnumerable<Expression> Expressions { get; }

        public SortType SortType { get; }
    }
}
