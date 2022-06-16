using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Dapper.Easies
{
    public static class EasiesProviderExtensions
    {
        private static TransactionScope CreateScope(ref TransactionOptions? options, TransactionScopeOption scopeOption)
        {
            return options == null ? new(scopeOption, TransactionScopeAsyncFlowOption.Enabled) : new(scopeOption, options.Value, TransactionScopeAsyncFlowOption.Enabled);
        }

        public static async Task TransactionScopeAsync(this IEasiesProvider _, Func<Task> func, TransactionOptions? options = null, TransactionScopeOption scopeOption = TransactionScopeOption.Required)
        {
            try
            {
                using (var scope = CreateScope(ref options, scopeOption))
                {
                    await func();
                    scope.Complete();
                }
            }
            catch (TransactionRollback.TransactionRollbackException)
            {
            }
        }

        public static async Task<TResult> TransactionScopeAsync<TResult>(this IEasiesProvider _, Func<Task<TResult>> func, TransactionOptions? options = null, TransactionScopeOption scopeOption = TransactionScopeOption.Required)
        {
            try
            {
                using (var scope = CreateScope(ref options, scopeOption))
                {
                    var result = await func();
                    scope.Complete();
                    return result;
                }
            }
            catch (TransactionRollback.TransactionRollbackException<TResult> ex)
            {
                return ex.ReturnValue;
            }
        }

        public static async Task TransactionScopeAsync(this IEasiesProvider _, Func<IRollback, Task> func, TransactionOptions? options = null, TransactionScopeOption scopeOption = TransactionScopeOption.Required)
        {
            try
            {
                using (var scope = CreateScope(ref options, scopeOption))
                {
                    await func(TransactionRollback.Instance);
                    scope.Complete();
                }
            }
            catch (TransactionRollback.TransactionRollbackException)
            {
            }
        }

        public static async Task<TResult> TransactionScopeAsync<TResult>(this IEasiesProvider _, Func<IRollbackWithResult, Task<TResult>> func, TransactionOptions? options = null, TransactionScopeOption scopeOption = TransactionScopeOption.Required)
        {
            try
            {
                using (var scope = CreateScope(ref options, scopeOption))
                {
                    var result = await func(TransactionRollback.Instance);
                    scope.Complete();
                    return result;
                }
            }
            catch (TransactionRollback.TransactionRollbackException<TResult> ex)
            {
                return ex.ReturnValue;
            }
        }
    }
}
