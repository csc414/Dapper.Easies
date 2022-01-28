using Dapper.Easies;
using Dapper.Easies.SqlServer;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EasiesOptionsExtensions
    {
        public static EasiesOptionsBuilder UseSqlServer(this EasiesOptionsBuilder options, string connectionString)
        {
            options.Options.ConnectionString = connectionString;
            options.Services.AddSingleton<IDbConnectionFactory, SqlServerDbConnectionFactory>();
            options.Services.Replace(new ServiceDescriptor(typeof(ISqlSyntax), typeof(SqlServerSqlSyntax), ServiceLifetime.Singleton));
            return options;
        }
    }
}
