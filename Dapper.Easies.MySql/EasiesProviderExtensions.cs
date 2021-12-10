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
            using (var scope = new TransactionScope(scopeOption, TransactionScopeAsyncFlowOption.Enabled))
            {
                func();
                scope.Complete();
            }
        }

        public static TResult TransactionScope<TResult>(this IEasiesProvider _, Func<TResult> func, TransactionScopeOption scopeOption = TransactionScopeOption.Required)
        {
            using (var scope = new TransactionScope(scopeOption, TransactionScopeAsyncFlowOption.Enabled))
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

        public static async Task<TResult> TransactionScopeAsync<TResult>(this IEasiesProvider _, Func<Task<TResult>> func, TransactionScopeOption scopeOption = TransactionScopeOption.Required)
        {
            using (var scope = new TransactionScope(scopeOption, TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await func();
                scope.Complete();
                return result;
            }
        }
    }
}
