using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DbTableAttribute : Attribute
    {
        public DbTableAttribute(string tableName)
        {
            TableName = tableName;
        }

        public string TableName { get; }
    }
}
