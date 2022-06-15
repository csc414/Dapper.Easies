using System;

namespace Dapper.Easies
{
    public interface IRollback
    {
        void Rollback();
    }

    public interface IRollbackWithResult
    {
        void Rollback<T>(T returnValue);
    }

    public sealed class TransactionRollback : IRollback, IRollbackWithResult
    {
        internal static TransactionRollback Instance { get; } = new TransactionRollback();

        private TransactionRollback() { }

        internal class TransactionRollbackException : Exception
        { 
        }

        internal class TransactionRollbackException<T> : TransactionRollbackException
        {
            public T ReturnValue { get; }

            public TransactionRollbackException(T returnValue)
            {
                ReturnValue = returnValue;
            }
        }

        public static void Rollback() => throw new TransactionRollbackException();

        public static void Rollback<T>(T returnValue) => throw new TransactionRollbackException<T>(returnValue);

        void IRollback.Rollback() => Rollback();

        void IRollbackWithResult.Rollback<T>(T returnValue) => Rollback(returnValue);
    }
}
