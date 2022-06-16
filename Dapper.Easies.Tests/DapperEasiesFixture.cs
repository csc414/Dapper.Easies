using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies.Tests
{
    public abstract class DapperEasiesFixture : IDapperEasiesFixture
    {
        public DapperEasiesFixture()
        {
            var services = new ServiceCollection();
            services.AddEasiesProvider(builder => Initialization(builder));
            ServiceProvider = services.BuildServiceProvider();
            EasiesProvider = ServiceProvider.GetRequiredService<IEasiesProvider>();
            SqlConverter = ServiceProvider.GetRequiredService<ISqlConverter>();
        }

        protected abstract void Initialization(EasiesOptionsBuilder builder);

        public IEasiesProvider EasiesProvider { get; }

        public IServiceProvider ServiceProvider { get; }

        public ISqlConverter SqlConverter { get; }
    }
}
