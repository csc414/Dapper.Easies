using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies
{
    public class MySqlDbFunc : DbFunc
    {
        public static decimal Rand()
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }
    }
}
