using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies
{
    public static class IDbQueryExtensions
    {
        public static T Skip<T>(this T query, int count) where T : IDbQuery
        {
            query.Context.Skip = count;
            return query;
        }

        public static T Take<T>(this T query, int count) where T : IDbQuery
        {
            query.Context.Take = count;
            return query;
        }
    }
}
