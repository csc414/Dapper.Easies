using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dapper.Easies
{
    internal class DefaultSqlConverter : ISqlConverter
    {
        private readonly ISqlSyntax _sqlSyntax;

        public DefaultSqlConverter(ISqlSyntax sqlSyntax)
        {
            _sqlSyntax = sqlSyntax;
            DbObject.Initialize(sqlSyntax);
        }

        public string ToQuerySql(QueryContext context, out DynamicParameters parameters)
        {
            var parameterBuilder = new ParameterBuilder(_sqlSyntax);
            var parser = new PredicateExpressionParser(_sqlSyntax, parameterBuilder);
            var sql = _sqlSyntax.QueryFormat(_sqlSyntax.TableNameAlias(context.Alias.Values.First()), GetFields(context), GetJoins(context, parser), GetPredicate(context, parser), null, context.Skip, context.Take);
            parameters = parameterBuilder.GetDynamicParameters();
            return sql;
        }

        string GetPredicate(QueryContext context, PredicateExpressionParser parser)
        {
            if(context.WhereExpressions.Count > 0)
                return string.Join(_sqlSyntax.Operator(OperatorType.AndAlso), context.WhereExpressions.Select(o => parser.ToSql(o, context)));
            return null;
        }

        string GetFields(QueryContext context)
        {
            if (context.JoinMetedatas.Count > 0)
                return "*";
            else
            {
                var alias = context.Alias[context.DbObject.Type];
                return string.Join(", ", context.DbObject.Properties.Select(o => $"{alias.Alias}.{o.EscapeNameAsAlias}"));
            }
        }

        IEnumerable<string> GetJoins(QueryContext context, PredicateExpressionParser parser)
        {
            if (context.JoinMetedatas.Count > 0)
                return context.JoinMetedatas.Select(o => _sqlSyntax.Join(_sqlSyntax.TableNameAlias(context.Alias[o.DbObject.Type]), o.Type, parser.ToSql(o.JoinExpression, context)));
            return null;
        }
    }
}
