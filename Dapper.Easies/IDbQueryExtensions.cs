using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
            var type = typeof(DbQuery<>).MakeGenericType(typeof(T).GetGenericArguments());
            return (T)Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { query.Context.Clone() }, null);
        }

        public static T Distinct<T>(this T query) where T : IDbQuery
        {
            query.Context.Distinct = true;
            return query;
        }

        public static async Task<(IEnumerable<T> data, long total, int max_page)> GetPagerAsync<T>(this ISelectedDbQuery<T> query, int page, int size)
        {
            var baseQuery = (IDbQuery<T>)query;
            var total = await baseQuery.CountAsync();
            var max_page = Convert.ToInt32(Math.Ceiling(total * 1f / size));
            if (page > max_page)
                return (Enumerable.Empty<T>(), total, max_page);

            var data = await baseQuery.Skip((page - 1) * size).Take(size).QueryAsync();
            return (data, total, max_page);
        }

        public static async Task<(IEnumerable<T> data, long total)> GetLimitAsync<T>(this ISelectedDbQuery<T> query, int skip, int take)
        {
            var baseQuery = (IDbQuery<T>)query;
            var total = await baseQuery.CountAsync();
            var data = await baseQuery.Skip(skip).Take(take).QueryAsync();
            return (data, total);
        }
    }
}
