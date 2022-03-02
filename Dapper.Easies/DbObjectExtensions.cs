using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Dapper.Easies
{
    public static class DbObjectExtensions
    {
        public static object Property(this IDbObject _, string name)
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }

        public static T Property<T>(this IDbObject _, string name)
        {
            throw new InvalidOperationException("Do not directly call this method.");
        }
    }
}
