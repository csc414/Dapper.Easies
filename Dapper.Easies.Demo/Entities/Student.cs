using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies.Demo
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

        public bool IsOk { get; set; }

        public DateTime CreateTime { get; set; }

        [DbProperty(Ignore = true)]
        public string ClassName { get; set; }
    }

    //[DbObject("bnt_student", ConnectionStringName = "MSSQL")]
    public class MStudent : IDbTable
    {
        [DbProperty(PrimaryKey = true, Identity = true)]
        public int Id { get; set; }

        public Guid ClassId { get; set; }

        [DbProperty("Name")]
        public string StudentName { get; set; }

        public int? Age { get; set; }

        public double? FloatTest { get; set; }

        public decimal? DecimalTest { get; set; }

        public bool IsOk { get; set; }

        public DateTime CreateTime { get; set; }

        [DbProperty(Ignore = true)]
        public string ClassName { get; set; }
    }
}
