using MySqlConnector;
using System.Data;

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
