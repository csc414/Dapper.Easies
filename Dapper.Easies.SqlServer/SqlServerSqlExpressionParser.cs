using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Dapper.Easies.SqlServer
{
    public class SqlServerSqlExpressionParser : SqlExpressionParser
    {
        public SqlServerSqlExpressionParser(QueryContext context) : base(context)
        {
        }

        protected override Expression AppendDateTimeMember(Expression instance, MemberInfo member)
        {
            switch (member.Name)
            {
                case "Date":
                    return new SqlExpression($"CONVERT(varchar(10), {GetPropertyName(instance)}, 23)");
                case "Hour":
                    return new SqlExpression($"DATEPART(HOUR, {GetPropertyName(instance)})");
                case "Minute":
                    return new SqlExpression($"DATEPART(MINUTE, {GetPropertyName(instance)})");
                case "Second":
                    return new SqlExpression($"DATEPART(SECOND, {GetPropertyName(instance)})");
                default:
                    return base.AppendDateTimeMember(instance, member);
            }
        }
    }
}
