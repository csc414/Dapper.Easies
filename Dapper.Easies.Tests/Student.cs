using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies.Tests
{
    public class Student : IDbTable
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid TeacherId { get; set; }

        public Guid ClassId { get; set; }
    }
}
