using Dapper.Easies;
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
            services.TryAddScoped<IEasiesProvider, DefaultEasiesProvider>();
            services.TryAddScoped<IDbConnectionCache, DefaultDbConnectionCache>();
            services.TryAddSingleton<ISqlConverter, DefaultSqlConverter>();
            services.TryAddSingleton(builder.Options);

            return services;
        }
    }
}
