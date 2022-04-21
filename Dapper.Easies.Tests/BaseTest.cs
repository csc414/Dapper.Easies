using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies.Tests
{
    public abstract class BaseTest
    {
        private readonly IDapperEasiesFixture _dapperEasiesFixture;

        public BaseTest(IDapperEasiesFixture dapperEasiesFixture)
        {
            _dapperEasiesFixture = dapperEasiesFixture;
        }

        protected IEasiesProvider EasiesProvider => _dapperEasiesFixture.EasiesProvider;

        protected ISqlConverter SqlConverter => _dapperEasiesFixture.SqlConverter;
    }
}
