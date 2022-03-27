using System;
using System.Collections.Generic;
using System.Data;

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
            if(factory == null)
                throw new ArgumentNullException(nameof(factory));

            return factory.Create();
        }
    }
}
