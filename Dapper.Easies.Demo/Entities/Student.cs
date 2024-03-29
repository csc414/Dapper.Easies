﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies.Demo
{
    [DbObject("bnt_student")]
    public class Student : IDbTable, ITenant
    {
        [DbProperty(PrimaryKey = true, Identity = true)]
        public int Id { get; set; }

        public Guid ClassId { get; set; }

        [DbProperty("StudentName")]
        public string Name { get; set; }

        public int? Age { get; set; }

        public bool IsAdult { get; set; }

        public DateTime CreateTime { get; set; }

        [DbProperty(Ignore = true)]
        public string ClassName { get; set; }

        public Guid? TenantId { get; set; }
    }
}
