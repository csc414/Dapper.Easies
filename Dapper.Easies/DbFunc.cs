﻿using System;
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

        public static T Min<T>(T field)
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }

        public static T Max<T>(T field)
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }

        public static T Avg<T>(T field)
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }

        public static T Avg<T>(object field)
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }

        public static T Sum<T>(T field)
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }

        public static T Sum<T>(object field)
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }

        public static T Expr<T>(string expr)
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }

        public static object Expr(string expr)
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

        public static T Count<T>()
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }

        public static T Count<T>(object field)
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }

        //TODO：实现SubQuery
        public static T SubQuery<T>(ISelectedDbQuery<T> query) where T : struct
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }

        public static string SubQuery(ISelectedDbQuery<string> query)
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }
    }
}
