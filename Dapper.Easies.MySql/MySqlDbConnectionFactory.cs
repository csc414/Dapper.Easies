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
        private readonly string _connectionString;

        public MySqlDbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection Create()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
