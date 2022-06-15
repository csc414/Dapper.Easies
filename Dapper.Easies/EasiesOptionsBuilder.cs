using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies
{
    public class EasiesOptionsBuilder
    {
        public static EasiesOptionsBuilder Instance { get; } = new EasiesOptionsBuilder();

        private EasiesOptions _options;

        private EasiesOptionsBuilder() { }

        /// <summary>
        /// 开发模式 会输出生成的 sql 语句
        /// </summary>
        /// <returns></returns>
        public EasiesOptionsBuilder DevelopmentMode()
        {
            Options.DevelopmentMode = true;
            return this;
        }

        public EasiesOptions Options => _options ?? (_options = new EasiesOptions());

        internal void SetOptions(EasiesOptions options)
        {
            _options = options;
        }
    }
}
