using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedSharp
{
    public static class Objects
    {
        public static bool EqualsAny(this Object obj, params Object[] vals)
        {
            return vals.Any(x => x == obj);
        }

        public static bool EqualsAny(this Object obj, IEnumerable<Object> vals)
        {
            return vals.Any(x => x == obj);
        }

        public static bool Null(this Object obj)
        {
            return obj == null;
        }

        public static bool NotNull(this Object obj)
        {
            return obj != null;
        }

        public static Func<A, bool> Eq<A>(this A x)
        {
            return y => Equals(x, y);
        }

        public static bool Is<A>(this Object x)
        {
            return x is A;
        }

        public static Func<Object, bool> Is<A>()
        {
            return x => x is A;
        }

        public static A As<A>(this Object x)
        {
            return (A)x;
        }

        public static Func<Object, A> As<A>()
        {
            return x => (A)x;
        }
    }
}
