using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DbPropertyAttribute : Attribute
    {
        public DbPropertyAttribute() { }

        public DbPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        /// <summary>
        /// 数据库字段名
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool PrimaryKey { get; set; }

        /// <summary>
        /// 是否自增长，在 PrimaryKey 等于 true 的情况下才生效
        /// </summary>
        public bool Identity { get; set; }
    }
}
