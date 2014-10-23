using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedSharp
{
    public static class Booleans
    {
        public static bool Not(this bool x)
        {
            return !x;
        }

        /// <summary>Does not short-circuit.</summary>
        public static bool And(this bool x, bool y)
        {
            return x & y;
        }

        /// <summary>Does not short-circuit.</summary>
        public static bool Or(this bool x, bool y)
        {
            return x | y;
        }

        /// <summary>Does not short-circuit.</summary>
        public static bool Xor(this bool x, bool y)
        {
            return x ^ y;
        }

        public static Func<A, bool> NotF<A>(this Func<A, bool> f)
        {
            return x => !f(x);
        }

        public static Func<A, bool> OrF<A>(params Func<A, bool>[] fs)
        {
            return x => fs.Any(f => f(x));
        }

        public static Func<A, bool> AndF<A>(params Func<A, bool>[] fs)
        {
            return x => fs.All(f => f(x));
        }

        public static Func<A, B, bool> OrF_<A, B>(Func<A, bool> fa, Func<B, bool> fb)
        {
            return (a, b) => fa(a) || fb(b);
        }

        public static Func<A, B, C, bool> OrF_<A, B, C>(Func<A, bool> fa, Func<B, bool> fb, Func<C, bool> fc)
        {
            return (a, b, c) => fa(a) || fb(b) || fc(c);
        }

        public static Func<A, B, bool> AndF_<A, B>(Func<A, bool> fa, Func<B, bool> fb)
        {
            return (a, b) => fa(a) && fb(b);
        }

        public static Func<A, B, C, bool> AndF_<A, B, C>(Func<A, bool> fa, Func<B, bool> fb, Func<C, bool> fc)
        {
            return (a, b, c) => fa(a) && fb(b) && fc(c);
        }
    }
}
