using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies
{
    public enum AggregateType : byte
    {
        Count,
        Max,
        Min,
        Avg,
        Sum
    }
}
