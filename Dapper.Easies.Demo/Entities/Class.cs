using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies.Demo
{
    [DbObject("bnt_class")]
    public class Class : IDbTable
    {
        [DbProperty(PrimaryKey = true)]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime CreateTime { get; set; }
    }

    public class TestClass
    {
        public string Name { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
