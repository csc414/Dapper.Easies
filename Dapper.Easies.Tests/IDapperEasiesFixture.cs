using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies.Tests
{
    public interface IDapperEasiesFixture
    {
        public IEasiesProvider EasiesProvider { get; }

        public IServiceProvider ServiceProvider { get; }

        public ISqlConverter SqlConverter { get; }
    }
}
