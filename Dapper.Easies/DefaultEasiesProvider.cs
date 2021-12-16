using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Linq;
using System.Linq.Expressions;

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

            var sql = _sqlConverter.ToInsertSql(entity, out var parameters, out var hasIdentity);
            if (hasIdentity)
            {
                var id = await Connection.ExecuteScalarAsync<long>(sql, parameters);
                if (id > 0)
                {
                    var propertyInfo = DbObject.Get(typeof(T)).IdentityKey.PropertyInfo;
                    var val = Convert.ChangeType(id, propertyInfo.PropertyType);
                    propertyInfo.SetValue(entity, val);
                    return true;
                }
                return false;
            }
            else
                return await Connection.ExecuteAsync(sql, parameters) > 0;
        }

        public Task<int> DeleteAsync<T>() where T : IDbTable
        {
            return DeleteAsync(new QueryContext(this, _sqlConverter, DbObject.Get(typeof(T))));
        }

        public Task<int> DeleteAsync<T>(Expression<Predicate<T>> predicate) where T : IDbTable
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var context = new QueryContext(this, _sqlConverter, DbObject.Get(typeof(T)));
            context.WhereExpressions.Add(predicate);
            return DeleteAsync(context);
        }

        public Task<int> DeleteAsync(IDbQuery query)
        {
            return DeleteAsync(query.Context);
        }

        public Task<int> DeleteCorrelationAsync(IDbQuery query)
        {
            return DeleteAsync(query.Context, true);
        }

        Task<int> DeleteAsync(QueryContext context, bool correlation = false)
        {
            var sql = _sqlConverter.ToDeleteSql(context, correlation, out var parameters);
            return Connection.ExecuteAsync(sql, parameters);
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
