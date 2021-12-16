using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Dapper.SqlMapper;

namespace Dapper.Easies
{
    public static class LoggerExtensions
    {
        internal static void LogQuerySql(this ILogger logger, string sql, DynamicParameters parameters)
        {
            IParameterLookup parameterLookup = parameters;
            logger.LogInformation($"Dapper.Easies Generate Sql：{Environment.NewLine}{string.Join(Environment.NewLine, parameters.ParameterNames.Select(name => $"set @{name} = {parameterLookup[name]};"))}{Environment.NewLine}{sql};");
        }

        internal static void LogInsertSql(this ILogger logger, string sql)
        {
            logger.LogInformation($"Dapper.Easies Generate Sql：{Environment.NewLine}{sql};");
        }
    }
}
