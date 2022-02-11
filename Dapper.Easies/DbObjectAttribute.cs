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

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// 连接字符串配置名 留空等同于 EasiesOptions.DefaultName = "Default";
        /// </summary>
        public string ConnectionStringName { get; set; }
    }
}
