using System;
using System.Linq.Expressions;

namespace Dapper.Easies
{
    public interface IDbObject { }

    public interface IDbView : IDbObject { }

    public interface IDbTable : IDbObject { }
}