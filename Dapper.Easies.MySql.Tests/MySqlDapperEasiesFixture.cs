using Dapper.Easies;
using Dapper.Easies.Tests;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies.MySql.Tests
{
    public class MySqlDapperEasiesFixture : DapperEasiesFixture
    {
        protected override void Initialization(EasiesOptionsBuilder builder)
        {
            builder.UseMySql(null);
        }
    }
}
