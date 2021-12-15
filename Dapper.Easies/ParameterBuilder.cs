using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies
{
    public class ParameterBuilder
    {
        private readonly ISqlSyntax _sqlSyntax;

        private readonly DynamicParameters _parameters = new DynamicParameters();
        
        private int i = 0;

        public ParameterBuilder(ISqlSyntax sqlSyntax)
        {
            _sqlSyntax = sqlSyntax;
        }

        public string AddParameter(object value)
        {
            string name = _sqlSyntax.ParameterName($"p{i++}");
            _parameters.Add(name, value);
            return name;
        }

        public DynamicParameters GetDynamicParameters() => _parameters;
    }
}
