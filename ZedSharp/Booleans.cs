using System;
using System.Linq;

namespace ZedSharp
{
    public static class Booleans
    {
        public static bool Not(this bool x)
        {
            return !x;
        }

        public static bool Implies(this bool x, bool y)
        {
            return !x || y;
        }

        public static Func<A, bool> Not<A>(this Func<A, bool> f)
        {
            return x => !f(x);
        }

        public static Func<A, bool> And<A>(this Func<A, bool> f, Func<A, bool> g)
        {
            return x => f(x) && g(x);
        }

        public static Func<A, bool> Or<A>(this Func<A, bool> f, Func<A, bool> g)
        {
            return x => f(x) || g(x);
        }
    }
}
