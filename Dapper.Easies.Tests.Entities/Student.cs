using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies.Tests
{
    [DbObject("bnt_student")]
    public class Student : IDbTable
    {
        [DbProperty(PrimaryKey = true, Identity = true)]
        public int Id { get; set; }

        public Guid ClassId { get; set; }

        [DbProperty("Name")]
        public string StudentName { get; set; }

        public int? Age { get; set; }

        public DateTime CreateTime { get; set; }

        [DbProperty(Ignore = true)]
        public string ClassName { get; set; }
    }
}
