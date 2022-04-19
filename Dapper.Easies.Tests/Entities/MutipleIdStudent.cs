using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies.Tests
{
    [DbObject("bnt_mutiple_id_students")]
    public class MutipleIdStudent : IDbTable
    {
        [DbProperty(PrimaryKey = true, Identity = true)]
        public int Id { get; set; }

        [DbProperty(PrimaryKey = true)]
        public int IdCard { get; set; }

        public Guid ClassId { get; set; }

        [DbProperty("StudentName")]
        public string Name { get; set; }

        public int? Age { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
