using Dapper.Easies;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

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
            var builder = new EasiesOptionsBuilder(services);
            builderAction?.Invoke(builder);
            services.Add(new ServiceDescriptor(typeof(IEasiesProvider), typeof(DefaultEasiesProvider), builder.Lifetime));
            services.AddSingleton<ISqlConverter, DefaultSqlConverter>();
            services.AddSingleton(builder.Options);
            if (!services.Any(o => o.ServiceType == typeof(ISqlSyntax)))
                services.AddSingleton<ISqlSyntax, DefaultSqlSyntax>();

            return services;
        }
    }
}
