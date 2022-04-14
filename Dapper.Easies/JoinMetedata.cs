using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
    public class JoinMetedata
    {
        public DbObject DbObject { get; set; }

        public JoinType Type { get; set; }

        public Expression JoinExpression { get; set; }

        public IDbQuery Query { get; set; }
    }
}
