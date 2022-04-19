using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Dapper.Easies.MySql.Tests
{
    [CollectionDefinition("MySql")]
    public class MySqlDapperEasiesCollection : ICollectionFixture<MySqlDapperEasiesFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
