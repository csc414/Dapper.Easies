using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Dapper.Easies.MySql
{
    public class MySqlExpressionParser : SqlExpressionParser
    {
        public MySqlExpressionParser(QueryContext context) : base(context)
        {
        }

        protected override Expression AppendDateTimeMember(Expression instance, MemberInfo member)
        {
            switch (member.Name)
            {
                case "Date":
                    return new SqlExpression($"DATE({GetPropertyName(instance)})");
                case "Hour":
                    return new SqlExpression($"HOUR({GetPropertyName(instance)})");
                case "Minute":
                    return new SqlExpression($"MINUTE({GetPropertyName(instance)})");
                case "Second":
                    return new SqlExpression($"SECOND({GetPropertyName(instance)})");
                default:
                    return base.AppendDateTimeMember(instance, member);
            }
        }
    }
}
