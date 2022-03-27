using Dapper.Easies;
using Dapper.Easies.MySql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EasiesOptionsExtensions
    {
        public static EasiesOptionsBuilder UseMySql(this EasiesOptionsBuilder options, string connectionString)
        {
            return options.UseMySql(EasiesOptions.DefaultName, connectionString);
        }

        public static EasiesOptionsBuilder UseMySql(this EasiesOptionsBuilder options, string name, string connectionString)
        {
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            options.Options.ConnectionFactory[name] = new MySqlDbConnectionFactory(connectionString);
            options.Options.SqlSyntax[name] = MySqlSqlSyntax.Instance;
            return options;
        }
    }
}
