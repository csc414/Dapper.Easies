using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Easies
{
    public interface IOrderedDbQuery<T> : IGeneralDbQuery<T>
    {
        IOrderedDbQuery<T> ThenBy(Expression<Func<T, object>> orderFields);

        IOrderedDbQuery<T> ThenByDescending(Expression<Func<T, object>> orderFields);
    }

    public interface IOrderedDbQuery<T1, T2> : IDbQuery<T1, T2>
    {
        IOrderedDbQuery<T1, T2> ThenBy(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2> ThenBy(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2> ThenByDescending(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2> ThenByDescending(Expression<Func<T1, T2, object>> orderFields);
    }

    public interface IOrderedDbQuery<T1, T2, T3> : IDbQuery<T1, T2, T3>
    {
        IOrderedDbQuery<T1, T2, T3> ThenBy(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3> ThenBy(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3> ThenBy(Expression<Func<T1, T2, T3, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3> ThenByDescending(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3> ThenByDescending(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3> ThenByDescending(Expression<Func<T1, T2, T3, object>> orderFields);
    }

    public interface IOrderedDbQuery<T1, T2, T3, T4> : IDbQuery<T1, T2, T3, T4>
    {
        IOrderedDbQuery<T1, T2, T3, T4> ThenBy(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4> ThenBy(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4> ThenBy(Expression<Func<T1, T2, T3, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4> ThenBy(Expression<Func<T1, T2, T3, T4, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4> ThenByDescending(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4> ThenByDescending(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4> ThenByDescending(Expression<Func<T1, T2, T3, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4> ThenByDescending(Expression<Func<T1, T2, T3, T4, object>> orderFields);
    }

    public interface IOrderedDbQuery<T1, T2, T3, T4, T5> : IDbQuery<T1, T2, T3, T4, T5>
    {
        IOrderedDbQuery<T1, T2, T3, T4, T5> ThenBy(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5> ThenBy(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5> ThenBy(Expression<Func<T1, T2, T3, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5> ThenBy(Expression<Func<T1, T2, T3, T4, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5> ThenBy(Expression<Func<T1, T2, T3, T4, T5, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5> ThenByDescending(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5> ThenByDescending(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5> ThenByDescending(Expression<Func<T1, T2, T3, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5> ThenByDescending(Expression<Func<T1, T2, T3, T4, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5> ThenByDescending(Expression<Func<T1, T2, T3, T4, T5, object>> orderFields);
    }

    public interface IOrderedDbQuery<T1, T2, T3, T4, T5, T6> : IDbQuery<T1, T2, T3, T4, T5, T6>
    {
        IOrderedDbQuery<T1, T2, T3, T4, T5, T6> ThenBy(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6> ThenBy(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6> ThenBy(Expression<Func<T1, T2, T3, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6> ThenBy(Expression<Func<T1, T2, T3, T4, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6> ThenBy(Expression<Func<T1, T2, T3, T4, T5, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6> ThenBy(Expression<Func<T1, T2, T3, T4, T5, T6, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6> ThenByDescending(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6> ThenByDescending(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6> ThenByDescending(Expression<Func<T1, T2, T3, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6> ThenByDescending(Expression<Func<T1, T2, T3, T4, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6> ThenByDescending(Expression<Func<T1, T2, T3, T4, T5, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6> ThenByDescending(Expression<Func<T1, T2, T3, T4, T5, T6, object>> orderFields);
    }

    public interface IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> : IDbQuery<T1, T2, T3, T4, T5, T6, T7>
    {
        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> ThenBy(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> ThenBy(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> ThenBy(Expression<Func<T1, T2, T3, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> ThenBy(Expression<Func<T1, T2, T3, T4, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> ThenBy(Expression<Func<T1, T2, T3, T4, T5, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> ThenBy(Expression<Func<T1, T2, T3, T4, T5, T6, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> ThenBy(Expression<Func<T1, T2, T3, T4, T5, T6, T7, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> ThenByDescending(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> ThenByDescending(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> ThenByDescending(Expression<Func<T1, T2, T3, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> ThenByDescending(Expression<Func<T1, T2, T3, T4, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> ThenByDescending(Expression<Func<T1, T2, T3, T4, T5, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> ThenByDescending(Expression<Func<T1, T2, T3, T4, T5, T6, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7> ThenByDescending(Expression<Func<T1, T2, T3, T4, T5, T6, T7, object>> orderFields);
    }

    public interface IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> : IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> ThenBy(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> ThenBy(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> ThenBy(Expression<Func<T1, T2, T3, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> ThenBy(Expression<Func<T1, T2, T3, T4, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> ThenBy(Expression<Func<T1, T2, T3, T4, T5, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> ThenBy(Expression<Func<T1, T2, T3, T4, T5, T6, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> ThenBy(Expression<Func<T1, T2, T3, T4, T5, T6, T7, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> ThenBy(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> ThenByDescending(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> ThenByDescending(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> ThenByDescending(Expression<Func<T1, T2, T3, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> ThenByDescending(Expression<Func<T1, T2, T3, T4, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> ThenByDescending(Expression<Func<T1, T2, T3, T4, T5, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> ThenByDescending(Expression<Func<T1, T2, T3, T4, T5, T6, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> ThenByDescending(Expression<Func<T1, T2, T3, T4, T5, T6, T7, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8> ThenByDescending(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, object>> orderFields);
    }

    public interface IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> : IDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> ThenBy(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> ThenBy(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> ThenBy(Expression<Func<T1, T2, T3, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> ThenBy(Expression<Func<T1, T2, T3, T4, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> ThenBy(Expression<Func<T1, T2, T3, T4, T5, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> ThenBy(Expression<Func<T1, T2, T3, T4, T5, T6, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> ThenBy(Expression<Func<T1, T2, T3, T4, T5, T6, T7, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> ThenBy(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> ThenBy(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> ThenByDescending(Expression<Func<T1, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> ThenByDescending(Expression<Func<T1, T2, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> ThenByDescending(Expression<Func<T1, T2, T3, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> ThenByDescending(Expression<Func<T1, T2, T3, T4, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> ThenByDescending(Expression<Func<T1, T2, T3, T4, T5, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> ThenByDescending(Expression<Func<T1, T2, T3, T4, T5, T6, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> ThenByDescending(Expression<Func<T1, T2, T3, T4, T5, T6, T7, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> ThenByDescending(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, object>> orderFields);

        IOrderedDbQuery<T1, T2, T3, T4, T5, T6, T7, T8, T9> ThenByDescending(Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, object>> orderFields);
    }
}
