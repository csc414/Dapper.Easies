using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Dapper.Easies
{
    public interface IConnection
    {
        IDbConnection Connection { get; }
    }
}
