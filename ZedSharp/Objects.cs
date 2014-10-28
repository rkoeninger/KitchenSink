using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedSharp
{
    public static class Objects
    {
        public static A To<A>(this IConvertible obj)
        {
            return (A)Convert.ChangeType(obj, typeof(A));
        }

        public static bool IsLessThan<A>(this A val, A that) where A : IComparable<A>
        {
            return val.CompareTo(that) < 0;
        }

        public static bool IsLessThanOrEquals<A>(this A val, A that) where A : IComparable<A>
        {
            return val.CompareTo(that) < 0;
        }

        public static bool IsGreaterThan<A>(this A val, A that) where A : IComparable<A>
        {
            return val.CompareTo(that) > 0;
        }

        public static bool IsGreaterThanOrEquals<A>(this A val, A that) where A : IComparable<A>
        {
            return val.CompareTo(that) >= 0;
        }

        /// <summary>Inclusive on lower bound, exclusive on upper bound.</summary>
        public static bool IsBetween<A>(this A val, A lower, A upper) where A : IComparable<A>
        {
            return val.IsGreaterThanOrEquals(lower) && val.IsLessThan(upper);
        }

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
    }
}
