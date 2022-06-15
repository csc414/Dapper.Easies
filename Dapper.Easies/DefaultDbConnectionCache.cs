using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Easies
{
    public class DefaultDbConnectionCache : IDbConnectionCache, IDisposable
    {
        private readonly EasiesOptions _options;

        private IDbConnection _connection;

        public DefaultDbConnectionCache(EasiesOptions options)
        {
            _options = options;
        }

        private Dictionary<IDbConnectionFactory, IDbConnection> _connections = new Dictionary<IDbConnectionFactory, IDbConnection>();

        public IDbConnection Connection => _connection ?? (_connection = GetConnection(EasiesOptions.DefaultName));

        public IDbConnection GetConnection(string connectionStringName)
        {
            return GetConnection(_options.GetConnectionFactory(connectionStringName));
        }

        public IDbConnection GetConnection(IDbConnectionFactory factory)
        {
            if (_connections.TryGetValue(factory, out var connection))
                return connection;
            
            _connections.Add(factory, connection = factory.Create());
            return connection ;
        }

        public IDbConnection CreateConnection(string connectionStringName)
        {
            return _options.GetConnectionFactory(connectionStringName).Create();
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

        public async Task<T> ExecuteAsync<T>(IDbConnectionFactory factory, Func<IDbConnection, Task<T>> func)
        {
            if (AsyncExecutionScope.IsAsync())
            {
                using (var conn = factory.Create())
                    return await func(conn);
            }

            return await func(GetConnection(factory));
        }
    }
}
