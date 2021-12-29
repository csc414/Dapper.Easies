using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies
{
    public struct DbAlias
    {
        public DbAlias(string name, string alias, bool isExpr = false)
        {
            Name = name;
            Alias = alias;
            IsExpr = isExpr;
        }

        public bool IsExpr { get; }

        public string Name { get; }

        public string Alias { get; }
    }
}
