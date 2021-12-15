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
        public static EasiesOptions AddMysql(this EasiesOptions options)
        {
            options.Services.AddSingleton<IDbConnectionFactory>(new MySqlDbConnectionFactory(options.ConnectionString));
            options.Services.Replace(new ServiceDescriptor(typeof(ISqlSyntax), typeof(MySqlSqlSyntax), ServiceLifetime.Singleton));
            return options;
        }
    }
}
