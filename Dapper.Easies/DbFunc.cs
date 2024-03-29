﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies
{
    public class DbFunc
    {
        public static bool Like<T>(T field, string text)
        {
            throw new InvalidOperationException("Do not call this method directly.");
        }

        public static bool In<T>(T field, IEnumerable<T> array)
        {
            throw new InvalidOperationException("Do not call this method directly.");
        }

        public static bool NotIn<T>(T field, IEnumerable<T> array)
        {
            throw new InvalidOperationException("Do not call this method directly.");
        }

        public static T Min<T>(T field)
        {
            throw new InvalidOperationException("Do not call this method directly.");
        }

        public static T Max<T>(T field)
        {
            throw new InvalidOperationException("Do not call this method directly.");
        }

        public static decimal Avg(object field)
        {
            throw new InvalidOperationException("Do not call this method directly.");
        }

        public static T Avg<T>(object field)
        {
            throw new InvalidOperationException("Do not call this method directly.");
        }

        public static T Sum<T>(T field)
        {
            throw new InvalidOperationException("Do not call this method directly.");
        }

        public static T Expr<T>(string expr)
        {
            throw new InvalidOperationException("Do not call this method directly.");
        }

        public static object Expr(string expr)
        {
            throw new InvalidOperationException("Do not call this method directly.");
        }

        public static long Count()
        {
            throw new InvalidOperationException("Do not call this method directly.");
        }

        public static long Count(object field)
        {
            throw new InvalidOperationException("Do not call this method directly.");
        }

        public static T Count<T>()
        {
            throw new InvalidOperationException("Do not call this method directly.");
        }

        public static T Count<T>(object field)
        {
            throw new InvalidOperationException("Do not call this method directly.");
        }

        public static bool IsNull<T>(T field) where T : unmanaged
        {
            throw new InvalidOperationException("Do not call this method directly.");
        }

        public static bool IsNotNull<T>(T field) where T : unmanaged
        {
            throw new InvalidOperationException("Do not call this method directly.");
        }

        public static T? N<T>(T field) where T : unmanaged
        {
            throw new InvalidOperationException("Do not call this method directly.");
        }
    }
}
