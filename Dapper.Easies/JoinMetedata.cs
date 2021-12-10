using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
    internal class JoinMetedata
    {
        public Type DbTable { get; set; }

        public JoinType Type { get; set; }

        public Expression JoinExpression { get; set; }
    }
}
