using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Easies
{
    public interface IDbQuery
    {
        public QueryContext Context { get; }
    }

    public interface IDbQuery<T> : IGeneralDbQuery<T>, IOperateDbQuery<T>
    {
    }

    public interface IDbQuery<T1, T2> : IGeneralDbQuery<T1, T2>
    {
    }

    public interface IDbQuery<T1, T2, T3> : IGeneralDbQuery<T1, T2, T3>
    {
    }

    public interface IDbQuery<T1, T2, T3, T4> : IGeneralDbQuery<T1, T2, T3, T4>
    {
    }

    public interface IDbQuery<T1, T2, T3, T4, T5> : IGeneralDbQuery<T1, T2, T3, T4, T5>
    {
    }

    public interface IDbQuery<T1, T2, T3, T4, T5, T6> : IGeneralDbQuery<T1, T2, T3, T4, T5, T6>
    {
    }

    public interface IDbQuery<T1, T2, T3, T4, T5, T6, T7> : IGeneralDbQuery<T1, T2, T3, T4, T5, T6, T7>
    {
    }

    public interface IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> : IGeneralDbQuery<T1, T2, T3, T4, T5, T6, T7, T8>
    {
    }

    public interface IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> : IGeneralDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
    }
}
