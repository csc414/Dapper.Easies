using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies
{
    public interface ISqlSyntax
    {
        string QueryFormat(string tableName, string fields, IEnumerable<string> joins, string where, string orderBy, int skip, int take);

        string InsertFormat(string tableName, IEnumerable<string> fields, IEnumerable<string> paramNames, bool hasIdentityKey);

        string DeleteFormat(IEnumerable<string> deleteTableAlias, string tableName, IEnumerable<string> joins, string where);

        string EscapeTableName(string name);

        string TableNameAlias(DbAlias alias);

        string EscapePropertyName(string name);

        string PropertyNameAlias(DbAlias alias);

        string ParameterName(string name);

        string Join(string tableName, JoinType joinType, string on);

        string Operator(OperatorType operatorType);

        string OrderBy(IEnumerable<string> orderBy, SortType orderBySortType, IEnumerable<string> thenBy, SortType? thenBySortType);
    }
}
