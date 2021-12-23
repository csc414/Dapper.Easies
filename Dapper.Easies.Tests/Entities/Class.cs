using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies.Tests
{
    [DbObject("bnt_class")]
    public class Class : IDbTable
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
