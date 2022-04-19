using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies.Tests
{
    [DbObject("bnt_classes")]
    public class Class : IDbTable
    {
        [DbProperty(PrimaryKey = true)]
        public Guid Id { get; set; }

        [DbProperty("ClassName")]
        public string Name { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
