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

        /// <summary>
        /// 根据配置名创建新的 DbConnection
        /// </summary>
        /// <param name="connectionStringName"></param>
        /// <returns></returns>
        IDbConnection CreateConnection(string connectionStringName);

        /// <summary>
        /// 执行上下文，自动判断异步环境
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        Task<T> ExecuteAsync<T>(IDbConnectionFactory factory, Func<IDbConnection, Task<T>> func);
    }
}
