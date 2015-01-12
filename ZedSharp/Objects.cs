using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ZedSharp
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNull(this Object obj)
        {
            return obj == null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNull(this Object obj)
        {
            return obj != null;
        }

        public static bool EqualsAny<A>(this A obj, params A[] vals)
        {
            return EqualsAny(obj, (IEnumerable<A>) vals);
        }

        public static bool EqualsAny<A>(this A obj, IEnumerable<A> vals)
        {
            return vals.Any(obj.Eq());
        }

        public static bool EqualsNone<A>(this A obj, params A[] vals)
        {
            return ! EqualsAny(obj, (IEnumerable<A>) vals);
        }

        public static bool EqualsNone<A>(this A obj, IEnumerable<A> vals)
        {
            return ! EqualsAny(obj, vals);
        }

        public static Func<A, bool> Eq<A>(this A x)
        {
            return y => Equals(x, y);
        }

        public static Func<A, bool> Same<A>(this A x)
        {
            return y => ReferenceEquals(x, y);
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
