using Microsoft.Data.Sqlite;
using System.Data;

namespace Dapper.Easies.Sqlite
{
    public class SqliteDbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqliteDbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection Create()
        {
            return new SqliteConnection(_connectionString);
        }
    }
}
