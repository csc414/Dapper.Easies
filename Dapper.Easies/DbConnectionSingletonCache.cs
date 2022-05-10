using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Dapper.Easies
{
    public class DbConnectionSingletonCache : IDbConnectionCache
    {
        private readonly EasiesOptions _options;

        public DbConnectionSingletonCache(EasiesOptions options)
        {
            _options = options;
        }

        public IDbConnection Connection => GetConnection(EasiesOptions.DefaultName);

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

            return factory.Create();
        }

        public Task ExecuteAsync(string connectionStringName, Func<IDbConnection, Task> func)
        {
            return ExecuteAsync(_options.GetConnectionFactory(connectionStringName), func);
        }

        public Task<T> ExecuteAsync<T>(string connectionStringName, Func<IDbConnection, Task<T>> func)
        {
            return ExecuteAsync(_options.GetConnectionFactory(connectionStringName), func);
        }

        public async Task ExecuteAsync(IDbConnectionFactory factory, Func<IDbConnection, Task> func)
        {
            using var conn = factory.Create();
            await func(conn);
        }

        public async Task<T> ExecuteAsync<T>(IDbConnectionFactory factory, Func<IDbConnection, Task<T>> func)
        {
            using var conn = factory.Create();
            return await func(conn);
        }
    }
}
