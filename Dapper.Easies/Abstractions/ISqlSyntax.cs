using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dapper.Easies
{
    public interface ISqlSyntax
    {
        string SelectFormat(QueryContext context, ParameterBuilder parameterBuilder, int? skip = null, int? take = null, AggregateInfo aggregateInfo = null);

        string SelectFormat(DbObject dbObject, object[] ids, ParameterBuilder parameterBuilder);

        string InsertFormat(DbObject dbObject, bool hasIdentityKey);

        string DeleteFormat(QueryContext context, ParameterBuilder parameterBuilder);

        string DeleteFormat(DbObject dbObject);

        string UpdateFormat(Expression fields, QueryContext context, ParameterBuilder parameterBuilder);

        string UpdateFormat(DbObject dbObject);

        string EscapeTableName(string name);

        string EscapePropertyName(string name);

        string AliasTableName(string name, string alias);

        string AliasPropertyName(string name, string alias);
    }
}
