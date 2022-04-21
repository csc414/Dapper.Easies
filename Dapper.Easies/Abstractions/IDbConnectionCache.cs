using System.Data;

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
    }
}
