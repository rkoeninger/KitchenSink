using System;
using System.Collections.Generic;
using System.Linq;

namespace KitchenSink
{
    public static class Objects
    {
        public static A To<A>(this IConvertible obj)
        {
            return (A)Convert.ChangeType(obj, typeof(A));
        }

        public static bool IsIn<A>(this A val, params A[] vals)
        {
            return IsIn(val, (ICollection<A>) vals);
        }

        public static bool IsIn<A>(this A val, ICollection<A> coll)
        {
            return coll.Any(val.Eq());
        }

        public static bool IsNotIn<A>(this A val, params A[] vals)
        {
            return ! IsIn(val, (ICollection<A>) vals);
        }

        public static bool IsNotIn<A>(this A val, ICollection<A> coll)
        {
            return ! IsIn(val, coll);
        }

        public static bool IsNull(this object obj)
        {
            return obj == null;
        }

        public static bool IsNotNull(this object obj)
        {
            return obj != null;
        }

        public static Func<A, bool> Eq<A>(this A x)
        {
            return y => Equals(x, y);
        }

        public static Func<A, bool> Same<A>(this A x)
        {
            return y => ReferenceEquals(x, y);
        }

        public static B With<A, B>(this A x, Func<A, B> f)
        {
            return x.IsNotNull() ? f(x) : default(B);
        }

        public static A With<A>(this A x, Action<A> f)
        {
            if (x.IsNotNull())
                f(x);

            return x;
        }

        public static B When<A, B>(this A x, Func<A, bool> p, Func<A, B> f)
        {
            return x.IsNotNull() && p(x) ? f(x) : default(B);
        }

        public static A When<A>(this A x, Func<A, bool> p, Action<A> f)
        {
            if (x.IsNotNull() && p(x))
                f(x);

            return x;
        }
    }
}
