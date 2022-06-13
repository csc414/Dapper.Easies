using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies
{
    public class ParameterBuilder
    {
        private readonly DynamicParameters _parameters = new DynamicParameters();
        
        private int i = 0;

        public string Add(object value)
        {
            string name = $"@p{i++}";
            _parameters.Add(name, value);
            return name;
        }

        public DynamicParameters GetDynamicParameters() => _parameters;
    }
}
