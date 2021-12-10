using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Dapper.Easies
{
    public class DefaultEasiesProvider : IEasiesProvider, IDisposable
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private IDbConnection _connection;

        public DefaultEasiesProvider(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public DbQuery<T> Table<T>() => new DbQuery<T>(new QueryContext(typeof(T), this));

        public IDbConnection Connection => _connection ?? (_connection = _connectionFactory.Create());

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
