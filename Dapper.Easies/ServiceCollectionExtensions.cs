﻿using Dapper.Easies;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("Dapper.Easies.Tests")]
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEasiesProvider(this IServiceCollection services)
        {
            return services.AddEasiesProvider(null);
        }

        public static IServiceCollection AddEasiesProvider(this IServiceCollection services, Action<EasiesOptionsBuilder> builderAction)
        {
            var builder = EasiesOptionsBuilder.Instance;
            builderAction?.Invoke(builder);
            services.TryAdd(new ServiceDescriptor(typeof(IEasiesProvider), typeof(DefaultEasiesProvider), builder.Lifetime));
            if (builder.Lifetime == ServiceLifetime.Singleton)
                services.TryAddSingleton<IDbConnectionCache, DbConnectionSingletonCache>();
            else
                services.TryAdd(new ServiceDescriptor(typeof(IDbConnectionCache), typeof(DbConnectionLifetimeCache), builder.Lifetime));
            services.TryAddSingleton<ISqlConverter, DefaultSqlConverter>();
            services.TryAddSingleton(builder.Options);

            return services;
        }
    }
}
