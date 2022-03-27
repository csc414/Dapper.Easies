using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies
{
    public class EasiesOptionsBuilder
    {
        private EasiesOptions _options;

        public EasiesOptionsBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }

        /// <summary>
        /// 开发模式 会输出生成的 sql 语句
        /// </summary>
        /// <returns></returns>
        public EasiesOptionsBuilder DevelopmentMode()
        {
            Options.DevelopmentMode = true;
            return this;
        }

        /// <summary>
        /// 当生命周期为 Transient 时，每次注入 IEasiesProvider 的实例将共享同一个 DbConnection.
        /// 当前生命周期为 Scoped 时，在整个请求生命周期中注入 IEasiesProvider 将共享同一个 DbConnection.
        /// 当生命周期为 Singleton 时，每次数据库的读写都会创建一个新的 DbConnection 实例.
        /// </summary>
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Scoped;

        public EasiesOptions Options => _options ?? (_options = new EasiesOptions());

        internal void SetOptions(EasiesOptions options)
        {
            _options = options;
        }
    }
}
