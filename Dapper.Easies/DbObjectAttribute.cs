using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DbObjectAttribute : Attribute
    {
        public DbObjectAttribute() { }

        public DbObjectAttribute(string tableName)
        {
            TableName = tableName;
        }

        public string TableName { get; set; }
    }
}
