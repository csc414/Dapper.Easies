using Microsoft.Data.SqlClient;
using System.Data;

namespace Dapper.Easies.SqlServer
{
    public class SqlServerDbConnectionFactory : IDbConnectionFactory
    {
        private readonly EasiesOptions _options;

        public SqlServerDbConnectionFactory(EasiesOptions options)
        {
            _options = options;
        }

        public IDbConnection Create()
        {
            return new SqlConnection(_options.ConnectionString);
        }
    }
}
