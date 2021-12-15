using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Linq;

namespace Dapper.Easies
{
    public class DefaultEasiesProvider : IEasiesProvider, IDisposable
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ISqlConverter _sqlConverter;

        private readonly ISqlSyntax _sqlSyntax;

        private IDbConnection _connection;

        public DefaultEasiesProvider(IDbConnectionFactory connectionFactory, ISqlConverter sqlConverter, ISqlSyntax sqlSyntax)
        {
            _connectionFactory = connectionFactory;
            _sqlConverter = sqlConverter;
            _sqlSyntax = sqlSyntax;
        }

        public IDbQuery<T> Query<T>() where T : IDbObject => new DbQuery<T>(new QueryContext(this, _sqlConverter, DbObject.Get(typeof(T))));

        public async Task<bool> InsertAsync<T>(T entity) where T : IDbTable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var table = DbObject.Get(typeof(T));
            var properties = table.Properties.Where(o => !o.IdentityKey);
            var sql = _sqlSyntax.InsertFormat(table.EscapeName, properties.Select(o => o.EscapeName), properties.Select(o => _sqlSyntax.ParameterName(o.PropertyInfo.Name)), table.IdentityKey != null);
            var parameters = new DynamicParameters(entity);
            if (table.IdentityKey == null)
                return await Connection.ExecuteAsync(sql, parameters) > 0;
            else
            {
                var id = await Connection.ExecuteScalarAsync<long>(sql, parameters);
                if (id > 0)
                {
                    table.IdentityKey.PropertyInfo.SetValue(entity, Convert.ChangeType(id, table.IdentityKey.PropertyInfo.PropertyType));
                    return true;
                }
                return false;
            }
        }

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
