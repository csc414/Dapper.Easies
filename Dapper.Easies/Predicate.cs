using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies
{
    public delegate bool Predicate<in T1, in T2>(T1 a, T2 b);

    public delegate bool Predicate<in T1, in T2, in T3>(T1 a, T2 b, T3 c);

    public delegate bool Predicate<in T1, in T2, in T3, in T4>(T1 a, T2 b, T3 c, T4 d);

    public delegate bool Predicate<in T1, in T2, in T3, in T4, in T5>(T1 a, T2 b, T3 c, T4 d, T5 e);
}
