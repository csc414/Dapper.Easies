using System;
using System.Data;
using System.Threading.Tasks;

namespace Dapper.Easies
{
    public interface IDbConnectionCache
    {
        /// <summary>
        /// Default 配置的 DbConnection，
        /// </summary>
        IDbConnection Connection { get; }

        /// <summary>
        /// 根据 IDbConnectionFactory 取 DbConnection
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        IDbConnection GetConnection(IDbConnectionFactory factory);

        /// <summary>
        /// 根据配置名取 DbConnection
        /// </summary>
        /// <param name="connectionStringName"></param>
        /// <returns></returns>
        IDbConnection GetConnection(string connectionStringName);

        Task ExecuteAsync(string connectionStringName, Func<IDbConnection, Task> func);

        Task<T> ExecuteAsync<T>(string connectionStringName, Func<IDbConnection, Task<T>> func);

        Task ExecuteAsync(IDbConnectionFactory factory, Func<IDbConnection, Task> func);

        Task<T> ExecuteAsync<T>(IDbConnectionFactory factory, Func<IDbConnection, Task<T>> func);
    }
}
