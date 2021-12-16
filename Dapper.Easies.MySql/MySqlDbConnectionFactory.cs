using Microsoft.Extensions.Options;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Dapper.Easies.MySql
{
    public class MySqlDbConnectionFactory : IDbConnectionFactory
    {
        private readonly EasiesOptions _options;

        public MySqlDbConnectionFactory(EasiesOptions options)
        {
            _options = options;
        }

        public IDbConnection Create()
        {
            return new MySqlConnection(_options.ConnectionString);
        }
    }
}
