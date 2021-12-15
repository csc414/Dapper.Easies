using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Dapper.Easies
{
    public interface IDbConnectionFactory
    {
        IDbConnection Create();
    }
}
