﻿using Dapper.Easies;
using Dapper.Easies.SqlServer;
using System;

namespace Dapper.Easies
{
    public static class EasiesOptionsExtensions
    {
        public static EasiesOptionsBuilder UseSqlServer(this EasiesOptionsBuilder options, string connectionString)
        {
            return options.UseSqlServer(EasiesOptions.DefaultName, connectionString);
        }

        public static EasiesOptionsBuilder UseSqlServer(this EasiesOptionsBuilder options, string name, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            options.Options.ConnectionFactory[name] = new SqlServerDbConnectionFactory(connectionString);
            options.Options.SqlSyntax[name] = SqlServerSqlSyntax.Instance;
            return options;
        }
    }
}
