using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies
{
    public struct DbAlias
    {
        public DbAlias(string name, string alias)
        {
            Name = name;
            Alias = alias;
        }

        public string Name { get; }

        public string Alias { get; }
    }
}
