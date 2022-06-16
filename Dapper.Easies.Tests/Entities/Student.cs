using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies.Tests
{
    [DbObject("tb_students")]
    public class Student : IDbTable
    {
        [DbProperty(PrimaryKey = true, Identity = true)]
        public int Id { get; set; }

        public Guid ClassId { get; set; }

        [DbProperty("StudentName")]
        public string Name { get; set; }

        public int? Age { get; set; }

        public DateTime CreateTime { get; set; }

        [DbProperty(Ignore = true)]
        public bool IsAdult => Age >= 18;
    }
}
