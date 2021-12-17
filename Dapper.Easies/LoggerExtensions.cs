﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Dapper.SqlMapper;

namespace Dapper.Easies
{
    public static class LoggerExtensions
    {
        internal static void LogParametersSql(this ILogger logger, string sql, DynamicParameters parameters)
        {
            IParameterLookup parameterLookup = parameters;
            logger.LogInformation($"Generated Sql：{Environment.NewLine}{string.Join(Environment.NewLine, parameters.ParameterNames.Select(name => $"set @{name} = {GetValue(parameterLookup[name])};"))}{Environment.NewLine}{sql};");
        }

        static string GetValue(object val)
        {
            if (val == null)
                return "null";

            if (val.GetType() == typeof(string))
                return $"\"{val}\"";

            return val.ToString();
        }

        internal static void LogSql(this ILogger logger, string sql)
        {
            logger.LogInformation($"Generated Sql：{Environment.NewLine}{sql};");
        }
    }
}
