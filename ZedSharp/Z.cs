using System;

namespace ZedSharp
{
    /// <summary>A collection of often useful, first-class functions.</summary>
    public static class Z
    {
        public static readonly Func<bool, bool> Not = x => !x;

        public static readonly Func<bool, bool, bool> And = (x, y) => x & y;

        public static readonly Func<bool, bool, bool> Or = (x, y) => x | y;

        public static readonly Func<bool, bool, bool> Xor = (x, y) => x ^ y;

        public static readonly Func<int, bool> Pos = x => x > 0;

        public static readonly Func<int, bool> Neg = x => x < 0;

        public static readonly Func<int, bool> NotNeg = x => x >= 0;

        public static readonly Func<int, bool> Even = x => x % 2 == 0;

        public static readonly Func<int, bool> Odd = x => x % 2 != 0;

        public static readonly Func<int, int, int> Add = (x, y) => x + y;

        public static readonly Func<int, int> Negate = x => -x;

        public static readonly Func<int, int> Inc = x => x + 1;

        public static readonly Func<int, int> Dec = x => x - 1;

        public static readonly Func<Object, Type> Type = x => x.GetType();

        public static readonly Func<Object, Object, bool> Eq = Equals;

        public static readonly Func<Object, Object, bool> Same = ReferenceEquals;

        public static readonly Func<Object, int> Hash = x => x == null ? 0 : x.GetHashCode();

        public static readonly Func<Object, String> Str = x => x == null ? "" : x.ToString();

        public static A Id<A>(A val)
        {
            return val;
        }

        /// <summary>Curried less than comparison.</summary>
        public static Func<A, bool> Lt<A>(A x) where A : IComparable<A>
        {
            return y => x.CompareTo(y) < 0;
        }
        
        /// <summary>Curried less than or equal to comparison.</summary>
        public static Func<A, bool> LtEq<A>(A x) where A : IComparable<A>
        {
            return y => x.CompareTo(y) <= 0;
        }
        
        /// <summary>Curried greater than comparison.</summary>
        public static Func<A, bool> Gt<A>(A x) where A : IComparable<A>
        {
            return y => x.CompareTo(y) > 0;
        }
        
        /// <summary>Curried greater than or equal to comparison.</summary>
        public static Func<A, bool> GtEq<A>(A x) where A : IComparable<A>
        {
            return y => x.CompareTo(y) >= 0;
        }
    }
}
