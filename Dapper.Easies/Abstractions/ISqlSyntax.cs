using System.Collections.Generic;

namespace Dapper.Easies
{
    public interface ISqlSyntax
    {
        string SelectFormat(QueryContext context, ParameterBuilder parameterBuilder, int? skip = null, int? take = null, AggregateInfo aggregateInfo = null);

        string InsertFormat(string tableName, IEnumerable<string> fields, IEnumerable<string> paramNames, bool hasIdentityKey);

        string DeleteFormat(string tableName, string tableAlias, IEnumerable<string> joins, string where);

        string UpdateFormat(string tableName, string tableAlias, IEnumerable<string> updateFields, string where);

        string EscapeTableName(string name);

        string EscapePropertyName(string name);

        string AliasTableName(string name, string alias);

        string AliasPropertyName(string name, string alias);
    }
}
