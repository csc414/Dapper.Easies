﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies.Tests
{
    public class Class : IDbTable
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
