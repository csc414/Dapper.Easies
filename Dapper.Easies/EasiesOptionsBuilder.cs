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

        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Scoped;

        public EasiesOptions Options => _options ?? (_options = new EasiesOptions());

        internal void SetOptions(EasiesOptions options)
        {
            _options = options;
        }
    }
}
