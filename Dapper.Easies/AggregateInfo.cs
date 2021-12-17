using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
    public class AggregateInfo
    {
        public AggregateInfo(AggregateType type, Expression expression)
        {
            Type = type;
            Expression = expression;
        }

        public AggregateType Type { get; }

        public Expression Expression { get; }
    }
}
