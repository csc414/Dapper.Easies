using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies
{
    public class DbFunc
    {
        public static bool Like<T>(T field, string text)
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }

        public static bool In<T>(T field, IEnumerable<T> array)
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }

        public static bool NotIn<T>(T field, IEnumerable<T> array)
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }

        public static long Count()
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }

        public static long Count<T>(T field)
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }

        public static T Min<T>(T field)
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }

        public static T Max<T>(T field)
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }

        public static long Avg<T>(T field)
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }

        public static long Sum<T>(T field)
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }

        public static T Expr<T>(string expr)
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }
    }
}
