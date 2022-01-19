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

        private IDbConnection _connection;

        public DefaultEasiesProvider(IDbConnectionFactory connectionFactory, ISqlConverter sqlConverter)
        {
            _connectionFactory = connectionFactory;
            _sqlConverter = sqlConverter;
        }

        public IDbQuery<T> Query<T>() where T : IDbObject => new DbQuery<T>(new QueryContext(this, _sqlConverter, DbObject.Get(typeof(T))));

        public Task<T> GetAsync<T>(params object[] ids) where T : IDbTable
        {
            var sql = _sqlConverter.ToGetSql<T>(ids, out var parameters);
            return Connection.QueryFirstOrDefaultAsync<T>(sql, parameters);
        }

        public async Task<bool> InsertAsync<T>(T entity) where T : IDbTable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var sql = _sqlConverter.ToInsertSql<T>(out var hasIdentity);
            if (hasIdentity)
            {
                var id = await Connection.ExecuteScalarAsync<long>(sql, entity);
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
                return await Connection.ExecuteAsync(sql, entity) > 0;
        }

        public Task<int> InsertAsync<T>(IEnumerable<T> entities) where T : IDbTable
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var sql = _sqlConverter.ToInsertSql<T>(out _);
            return Connection.ExecuteAsync(sql, entities);
        }

        public Task<int> DeleteAsync<T>(T entity) where T : IDbTable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var sql = _sqlConverter.ToDeleteSql<T>();
            return Connection.ExecuteAsync(sql, entity);
        }

        public Task<int> DeleteAsync<T>(IEnumerable<T> entities) where T : IDbTable
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var sql = _sqlConverter.ToDeleteSql<T>();
            return Connection.ExecuteAsync(sql, entities);
        }

        public Task<int> DeleteAsync<T>(Expression<Predicate<T>> predicate = null) where T : IDbTable
        {
            var context = new QueryContext(this, _sqlConverter, DbObject.Get(typeof(T)));
            if (predicate != null)
                context.AddWhere(predicate);

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

        public Task<int> UpdateAsync<T>(Expression<Func<T>> updateFields, Expression<Predicate<T>> predicate = null) where T : IDbTable
        {
            return InternalUpdateAsync(updateFields, predicate);
        }

        public Task<int> UpdateAsync<T>(Expression<Func<T, T>> updateFields, Expression<Predicate<T>> predicate = null) where T : IDbTable
        {
            return InternalUpdateAsync(updateFields, predicate);
        }

        Task<int> InternalUpdateAsync<T>(Expression updateFields, Expression<Predicate<T>> predicate = null) where T : IDbTable
        {
            var context = new QueryContext(this, _sqlConverter, DbObject.Get(typeof(T)));
            if (predicate != null)
                context.AddWhere(predicate);

            var sql = _sqlConverter.ToUpdateFieldsSql(updateFields, context, out var parameters);
            return Connection.ExecuteAsync(sql, parameters);
        }

        public Task<int> UpdateAsync<T>(T entity) where T : IDbTable
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var sql = _sqlConverter.ToUpdateSql<T>();
            return Connection.ExecuteAsync(sql, entity);
        }

        public Task<int> UpdateAsync<T>(IEnumerable<T> entities) where T : IDbTable
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            var sql = _sqlConverter.ToUpdateSql<T>();
            return Connection.ExecuteAsync(sql, entities);
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
