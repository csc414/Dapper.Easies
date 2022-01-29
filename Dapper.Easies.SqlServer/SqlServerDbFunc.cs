using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies
{
    public class SqlServerDbFunc : DbFunc
    {
        public static int Count()
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }

        public static int Count<T>(T field)
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }
    }
}
