using Dapper.Easies;
using Dapper.Easies.MySql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies
{
    public static class EasiesOptionsExtensions
    {
        public static EasiesOptionsBuilder AddMysql(this EasiesOptionsBuilder options, string connectionString)
        {
            options.Options.ConnectionString = connectionString;
            options.Services.AddSingleton<IDbConnectionFactory, MySqlDbConnectionFactory>();
            options.Services.Replace(new ServiceDescriptor(typeof(ISqlSyntax), typeof(MySqlSqlSyntax), ServiceLifetime.Singleton));
            return options;
        }
    }
}
