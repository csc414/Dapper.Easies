using System;
using System.Threading;

namespace Dapper.Easies
{
    public sealed class AsyncExecutionScope : IDisposable
    {
        private static readonly AsyncLocal<bool?> _locals = new AsyncLocal<bool?>();

        internal static bool IsAsync() => _locals.Value == true;

        /// <summary>
        /// 创建异步执行范围
        /// </summary>
        /// <returns></returns>
        public static AsyncExecutionScope Create()
        {
            return new AsyncExecutionScope();
        }

        private AsyncExecutionScope()
        {
            _locals.Value = true;
        }

        public void Dispose()
        {
            _locals.Value = null;
        }
    }
}
