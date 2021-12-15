using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies
{
    public class EasiesOptions
    {
        public EasiesOptions(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }

        public string ConnectionString { get; set; }

        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Scoped;
    }
}
