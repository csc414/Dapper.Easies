using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
    public class JoinMetedata
    {
        public JoinMetedata(DbObject dbObject, JoinType type, Expression joinExpression, IDbQuery query)
        {
            DbObject = dbObject;
            Type = type;
            JoinExpression = joinExpression;
            Query = query;
        }

        public DbObject DbObject { get; }

        public JoinType Type { get; }

        public Expression JoinExpression { get; }

        public IDbQuery Query { get; }
    }
}
