﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Dapper.Easies
{
    public interface ISqlSyntax
    {
        string SelectFormat(string tableName, IEnumerable<string> fields, IEnumerable<string> joins, string where, string orderBy, int skip, int take);

        string InsertFormat(string tableName, IEnumerable<string> fields, IEnumerable<string> paramNames, bool hasIdentityKey);

        string DeleteFormat(string tableName, IEnumerable<string> deleteTableAlias, IEnumerable<string> joins, string where);

        string UpdateFormat(string tableName, IEnumerable<string> updateFields, string where);

        string EscapeTableName(string name);

        string TableNameAlias(DbAlias alias);

        string EscapePropertyName(string name);

        string PropertyNameAlias(DbAlias alias);

        string ParameterName(string name);

        string Join(string tableName, JoinType joinType, string on);

        string Operator(OperatorType operatorType);

        string OrderBy(IEnumerable<string> orderBy, SortType orderBySortType, IEnumerable<string> thenBy, SortType? thenBySortType);

        string Method(MethodInfo method, string field, object[] args, ParameterBuilder parameter);
    }
}
