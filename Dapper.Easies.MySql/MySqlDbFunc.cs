﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies
{
    public class MySqlDbFunc : DbFunc
    {
        public static double Rand()
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
    }
}
