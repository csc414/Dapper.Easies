using System;
using System.Data;

namespace Dapper.Easies
{
    internal class UnmanagedTypeHandler<T> : SqlMapper.TypeHandler<T> where T : unmanaged
    {
        public override T Parse(object value)
        {
            if (value is T num)
                return num;

            var val = Convert.ChangeType(value, typeof(T));
            if (val != null)
                return (T)val;

            return default;
        }

        public override void SetValue(IDbDataParameter parameter, T value)
        {
            parameter.Value = value;
        }
    }
}
