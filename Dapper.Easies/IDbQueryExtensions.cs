using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Easies
{
    public static class DbQueryExtensions
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

        /// <summary>
        /// Do not call this method directly.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static T SubQueryScalar<T>(this ISelectedDbQuery<T> _) where T : struct
        {
            throw new InvalidOperationException("Do not call this method directly.");
        }

        /// <summary>
        /// Do not call this method directly.
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static string SubQueryScalar(this ISelectedDbQuery<string> _)
        {
            throw new InvalidOperationException("Do not call this method directly.");
        }

        /// <summary>
        /// Do not call this method directly.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static IEnumerable<T> SubQuery<T>(this ISelectedDbQuery<T> _) where T : struct
        {
            throw new InvalidOperationException("Do not call this method directly.");
        }

        /// <summary>
        /// Do not call this method directly.
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static IEnumerable<string> SubQuery(this ISelectedDbQuery<string> _)
        {
            throw new InvalidOperationException("Do not call this method directly.");
        }

        public static IDbQuery<T> WhereIf<T>(this IDbQuery<T> query, bool condition, Expression<Func<T, bool>> ifTrue, Expression<Func<T, bool>> ifFalse = null)
        {
            if (condition)
                return query.Where(ifTrue);
            else if (ifFalse != null)
                return query.Where(ifFalse);
            return query;
        }

        public static IDbQuery<T1, T2> WhereIf<T1, T2>(this IDbQuery<T1, T2> query, bool condition, Expression<Func<T1, T2, bool>> ifTrue, Expression<Func<T1, T2, bool>> ifFalse = null)
        {
            if (condition)
                return query.Where(ifTrue);
            else if (ifFalse != null)
                return query.Where(ifFalse);
            return query;
        }

        public static IDbQuery<T1, T2, T3> WhereIf<T1, T2, T3>(this IDbQuery<T1, T2, T3> query, bool condition, Expression<Func<T1, T2, T3, bool>> ifTrue, Expression<Func<T1, T2, T3, bool>> ifFalse = null)
        {
            if (condition)
                return query.Where(ifTrue);
            else if (ifFalse != null)
                return query.Where(ifFalse);
            return query;
        }

        public static IDbQuery<T1, T2, T3, T4> WhereIf<T1, T2, T3, T4>(this IDbQuery<T1, T2, T3, T4> query, bool condition, Expression<Func<T1, T2, T3, T4, bool>> ifTrue, Expression<Func<T1, T2, T3, T4, bool>> ifFalse = null)
        {
            if (condition)
                return query.Where(ifTrue);
            else if (ifFalse != null)
                return query.Where(ifFalse);
            return query;
        }

        public static IDbQuery<T1, T2, T3, T4, T5> WhereIf<T1, T2, T3, T4, T5>(this IDbQuery<T1, T2, T3, T4, T5> query, bool condition, Expression<Func<T1, T2, T3, T4, T5, bool>> ifTrue, Expression<Func<T1, T2, T3, T4, T5, bool>> ifFalse = null)
        {
            if (condition)
                return query.Where(ifTrue);
            else if (ifFalse != null)
                return query.Where(ifFalse);
            return query;
        }
    }
}
