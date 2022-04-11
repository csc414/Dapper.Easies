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
    public class DefaultEasiesProvider : IEasiesProvider
    {
        private readonly IDbConnectionCache _connection;

        private readonly ISqlConverter _sqlConverter;

        public DefaultEasiesProvider(IDbConnectionCache connection, ISqlConverter sqlConverter)
        {
            _connection = connection;
            _sqlConverter = sqlConverter;
        }

        public DbEntity<T> Entity<T>() where T : IDbTable => new DbEntity<T>(this);

        public IDbQuery<T> From<T>() where T : IDbObject => new DbQuery<T>(new QueryContext(_connection, _sqlConverter, DbObject.Get(typeof(T))));

        public Task<T> GetAsync<T>(params object[] ids) where T : IDbObject
        {
            var sql = _sqlConverter.ToGetSql<T>(ids, out var parameters);
            return GetConnection<T>().QueryFirstOrDefaultAsync<T>(sql, parameters);
        }

        public async Task<bool> InsertAsync<T>(T entity) where T : IDbTable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var sql = _sqlConverter.ToInsertSql<T>(out var hasIdentity);
            if (hasIdentity)
            {
                var id = await GetConnection<T>().ExecuteScalarAsync<long>(sql, entity);
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
                return await GetConnection<T>().ExecuteAsync(sql, entity) > 0;
        }

        public Task<int> InsertAsync<T>(IEnumerable<T> entities) where T : IDbTable
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var sql = _sqlConverter.ToInsertSql<T>(out _);
            return GetConnection<T>().ExecuteAsync(sql, entities);
        }

        public Task<int> DeleteAsync<T>(T entity) where T : IDbTable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var sql = _sqlConverter.ToDeleteSql<T>();
            return GetConnection<T>().ExecuteAsync(sql, entity);
        }

        public Task<int> DeleteAsync<T>(IEnumerable<T> entities) where T : IDbTable
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var sql = _sqlConverter.ToDeleteSql<T>();
            return GetConnection<T>().ExecuteAsync(sql, entities);
        }

        public Task<int> UpdateAsync<T>(T entity) where T : IDbTable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var sql = _sqlConverter.ToUpdateSql<T>();
            return GetConnection<T>().ExecuteAsync(sql, entity);
        }

        public Task<int> UpdateAsync<T>(IEnumerable<T> entities) where T : IDbTable
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var sql = _sqlConverter.ToUpdateSql<T>();
            return GetConnection<T>().ExecuteAsync(sql, entities);
        }

        IDbConnection GetConnection<T>() => _connection.GetConnection(DbObject.Get<T>()?.ConnectionFactory);

        public IDbConnection Connection => _connection.Connection;

        public IDbConnection GetConnection(string connectionStringName) => _connection.GetConnection(connectionStringName);
    }
}
