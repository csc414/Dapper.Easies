using Dapper.Easies;
using Dapper.Easies.MySql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEasiesMysql(this IServiceCollection services, string connectionString)
        {
            return services.AddSingleton<IDbConnectionFactory>(new MySqlDbConnectionFactory(connectionString));
        }
    }
}
