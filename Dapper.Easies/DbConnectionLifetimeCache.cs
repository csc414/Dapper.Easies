using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Dapper.Easies
{
    public class DbConnectionLifetimeCache : IDbConnectionCache, IDisposable
    {
        private readonly EasiesOptions _options;

        private IDbConnection _connection;

        public DbConnectionLifetimeCache(EasiesOptions options)
        {
            _options = options;
        }

        private Dictionary<IDbConnectionFactory, IDbConnection> _connections = new Dictionary<IDbConnectionFactory, IDbConnection>();

        public IDbConnection Connection => _connection ?? (_connection = GetConnection(EasiesOptions.DefaultName));

        public IDbConnection GetConnection(string connectionStringName)
        {
            if (connectionStringName == null)
                connectionStringName = EasiesOptions.DefaultName;

            return GetConnection(_options.GetConnectionFactory(connectionStringName));
        }

        public IDbConnection GetConnection(IDbConnectionFactory factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            if (_connections.TryGetValue(factory, out var connection))
                return connection;
            
            _connections.Add(factory, connection = factory.Create());
            return connection ;
        }

        public void Dispose()
        {
            _connection = null;

            if (_connections.Count > 0)
            {
                foreach (var connection in _connections)
                    connection.Value.Dispose();

                _connections.Clear();
            }
        }
    }
}
