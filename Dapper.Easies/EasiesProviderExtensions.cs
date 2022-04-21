using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Dapper.Easies
{
    public static class EasiesProviderExtensions
    {
        public static void TransactionScope(this IEasiesProvider _, Action func, TransactionScopeOption scopeOption = TransactionScopeOption.Required)
        {
            using (var scope = new TransactionScope(scopeOption))
            {
                func();
                scope.Complete();
            }
        }

        public static void TransactionScope(this IEasiesProvider _, Action func, TransactionOptions options, TransactionScopeOption scopeOption = TransactionScopeOption.Required)
        {
            using (var scope = new TransactionScope(scopeOption, options))
            {
                func();
                scope.Complete();
            }
        }

        public static TResult TransactionScope<TResult>(this IEasiesProvider _, Func<TResult> func, TransactionScopeOption scopeOption = TransactionScopeOption.Required)
        {
            using (var scope = new TransactionScope(scopeOption))
            {
                var result = func();
                scope.Complete();
                return result;
            }
        }

        public static TResult TransactionScope<TResult>(this IEasiesProvider _, Func<TResult> func, TransactionOptions options, TransactionScopeOption scopeOption = TransactionScopeOption.Required)
        {
            using (var scope = new TransactionScope(scopeOption, options))
            {
                var result = func();
                scope.Complete();
                return result;
            }
        }

        public static async Task TransactionScopeAsync(this IEasiesProvider _, Func<Task> func, TransactionScopeOption scopeOption = TransactionScopeOption.Required)
        {
            using (var scope = new TransactionScope(scopeOption, TransactionScopeAsyncFlowOption.Enabled))
            {
                await func();
                scope.Complete();
            }
        }

        public static async Task TransactionScopeAsync(this IEasiesProvider _, Func<Task> func, TransactionOptions options, TransactionScopeOption scopeOption = TransactionScopeOption.Required)
        {
            using (var scope = new TransactionScope(scopeOption, options, TransactionScopeAsyncFlowOption.Enabled))
            {
                await func();
                scope.Complete();
            }
        }

        public static async Task<TResult> TransactionScopeAsync<TResult>(this IEasiesProvider _, Func<Task<TResult>> func, TransactionScopeOption scopeOption = TransactionScopeOption.Required)
        {
            using (var scope = new TransactionScope(scopeOption, TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await func();
                scope.Complete();
                return result;
            }
        }

        public static async Task<TResult> TransactionScopeAsync<TResult>(this IEasiesProvider _, Func<Task<TResult>> func, TransactionOptions options, TransactionScopeOption scopeOption = TransactionScopeOption.Required)
        {
            using (var scope = new TransactionScope(scopeOption, options, TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await func();
                scope.Complete();
                return result;
            }
        }
    }
}
