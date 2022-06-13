using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies.MySql
{
    public class MySqlExpressionParser : SqlExpressionParser
    {
        public MySqlExpressionParser(QueryContext context) : base(context)
        {
        }
    }
}
