using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies
{
    public interface IDbMapper
    {
        void SetTableName<T>(string tableName) where T : IDbObject;
    }
}
