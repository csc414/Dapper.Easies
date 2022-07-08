using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Dapper.Easies
{
    public class EasiesOptions
    {
        public const string DefaultName = "Default";

        public IDictionary<string, IDbConnectionFactory> ConnectionFactory { get; set; } = new Dictionary<string, IDbConnectionFactory>(StringComparer.Ordinal);

        public IDictionary<string, ISqlSyntax> SqlSyntax { get; set; } = new Dictionary<string, ISqlSyntax>(StringComparer.Ordinal);

        public bool DevelopmentMode { get; set; }

        public IList<Type> Appenders { get; } = new List<Type>();

        public IDbConnectionFactory GetConnectionFactory(string connectionStringName)
        {
            if (ConnectionFactory.TryGetValue(connectionStringName, out var factory))
                return factory;

            throw new ArgumentException($"Invalid ConnectionStringName：{connectionStringName}", nameof(connectionStringName));
        }

        public IDbConnection GetConnection(string connectionStringName)
        {
            if (ConnectionFactory.TryGetValue(connectionStringName, out var factory))
                return factory.Create();

            throw new ArgumentException($"Invalid ConnectionStringName：{connectionStringName}", nameof(connectionStringName));
        }

        public ISqlSyntax GetSqlSyntax(string connectionStringName)
        {
            if (SqlSyntax.TryGetValue(connectionStringName ?? DefaultName, out var sqlSyntax))
                return sqlSyntax;

            throw new ArgumentException($"Invalid ConnectionStringName：{connectionStringName}", nameof(connectionStringName));
        }
    }
}
