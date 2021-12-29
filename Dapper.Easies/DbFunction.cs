using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies
{
    public class DbFunction
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

        public static T Expression<T>(string expr)
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }
    }
}
