using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Dapper.Easies
{
    public interface ISqlSyntax
    {
        string SelectFormat(string tableName, IEnumerable<string> fields, IEnumerable<string> joins, string where, string groupBy, string having, string orderBy, int skip, int take, bool distinct);

        string InsertFormat(string tableName, IEnumerable<string> fields, IEnumerable<string> paramNames, bool hasIdentityKey);

        string DeleteFormat(string tableName, string tableAlias, IEnumerable<string> joins, string where);

        string UpdateFormat(string tableName, string tableAlias, IEnumerable<string> updateFields, string where);

        string EscapeTableName(string name);

        string TableNameAlias(DbAlias alias);

        string EscapePropertyName(string name);

        string PropertyNameAlias(DbAlias alias);

        string ParameterName(string name);

        string Join(string tableName, JoinType joinType, string on);

        string Operator(OperatorType operatorType);

        string OrderBy(IEnumerable<string> orderBy, SortType orderBySortType, IEnumerable<string> thenBy, SortType? thenBySortType);

        string GroupBy(IEnumerable<string> groupBy);

        string Method(string methodName, Expression[] args, ParameterBuilder parameter, Func<Expression, string> getExpr, Func<Expression, object> getValue);

        string DateTimeMethod(string name, Func<string> getPropertyName);
    }
}
