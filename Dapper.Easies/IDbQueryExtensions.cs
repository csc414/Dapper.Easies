using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
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

        public static T NewQuery<T>(this T query) where T : IDbQuery
        {
            if (query.Context == null)
                return query;

            var type = typeof(DbQuery<>).MakeGenericType(typeof(T).GetGenericArguments());
            return (T)Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { query.Context.Clone() }, null);
        }

        public static T Distinct<T>(this T query) where T : IDbQuery
        {
            query.Context.Distinct = true;
            return query;
        }
    }
}
