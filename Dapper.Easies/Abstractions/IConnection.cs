using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Dapper.Easies
{
    public interface IConnection
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
    }
}
