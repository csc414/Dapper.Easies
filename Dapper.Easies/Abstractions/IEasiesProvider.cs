using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Dapper.Easies
{
    public interface IEasiesProvider
    {
        /// <summary>
        /// Default 配置的 DbConnection，
        /// </summary>
        IDbConnection Connection { get; }

        /// <summary>
        /// 根据配置名取 DbConnection
        /// </summary>
        /// <param name="connectionStringName"></param>
        /// <returns></returns>
        IDbConnection GetConnection(string connectionStringName);

        /// <summary>
        /// 根据 Default 配置创建一个新的 DbConnection
        /// </summary>
        /// <returns></returns>
        IDbConnection CreateConnection();

        /// <summary>
        /// 根据配置名创建一个新的 DbConnection
        /// </summary>
        /// <param name="connectionStringName"></param>
        /// <returns></returns>
        IDbConnection CreateConnection(string connectionStringName);

        /// <summary>
        /// 强类型实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        DbEntity<T> Entity<T>() where T : IDbTable;

        /// <summary>
        /// 高级查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IDbQuery<T> From<T>() where T : IDbObject;

        /// <summary>
        /// 根据主键获取实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids">主键，若复合主键请参考模型中主键的顺序依次传入</param>
        /// <returns></returns>
        Task<T> GetAsync<T>(params object[] ids) where T : IDbObject;

        /// <summary>
        /// 新增实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> InsertAsync<T>(T entity) where T : IDbTable;

        /// <summary>
        /// 批量新增实体对象，批量新增无法回填实体自增Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task<int> InsertAsync<T>(IEnumerable<T> entities) where T : IDbTable;

        /// <summary>
        /// 删除实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> DeleteAsync<T>(T entity) where T : IDbTable;

        /// <summary>
        /// 批量删除实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task<int> DeleteAsync<T>(IEnumerable<T> entities) where T : IDbTable;

        /// <summary>
        /// 更新实体对象除主键外的所有字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> UpdateAsync<T>(T entity) where T : IDbTable;

        /// <summary>
        /// 批量更新实体对象除主键外的所有字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task<int> UpdateAsync<T>(IEnumerable<T> entities) where T : IDbTable;
    }
}
