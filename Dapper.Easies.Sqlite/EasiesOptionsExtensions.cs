using Dapper;
using Dapper.Easies;
using Dapper.Easies.MySql;
using Dapper.Easies.Sqlite;
using System;

namespace Dapper.Easies
{
    public static class EasiesOptionsExtensions
    {
        static EasiesOptionsExtensions()
        {
            SqlMapper.AddTypeHandler(new DateTimeOffsetHandler());
            SqlMapper.AddTypeHandler(new GuidHandler());
            SqlMapper.AddTypeHandler(new TimeSpanHandler());
        }

        public static EasiesOptionsBuilder UseSqlite(this EasiesOptionsBuilder options, string connectionString)
        {
            return options.UseSqlite(EasiesOptions.DefaultName, connectionString);
        }

        public static EasiesOptionsBuilder UseSqlite(this EasiesOptionsBuilder options, string name, string connectionString)
        {
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            options.Options.ConnectionFactory[name] = new SqliteDbConnectionFactory(connectionString);
            options.Options.SqlSyntax[name] = SqliteSqlSyntax.Instance;
            return options;
        }
    }
}
