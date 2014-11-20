using System;
using System.Collections.Generic;
using System.Linq;

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

        public static readonly Func<int, bool> Even = x => x % 2 == 0;

        public static readonly Func<int, bool> Odd = x => x % 2 != 0;

        public static readonly Func<int, int, int> Add = (x, y) => x + y;

        public static readonly Func<int, int> Negate = x => -x;

        public static readonly Func<int, int> Inc = x => x + 1;

        public static readonly Func<int, int> Dec = x => x - 1;

        public static readonly Func<Object, Type> Type = x => x.GetType();

        public static readonly Func<Object, Object, bool> Eq = Object.Equals;

        public static readonly Func<Object, Object, bool> Same = Object.ReferenceEquals;

        public static readonly Func<Object, int> Hash = x => x == null ? 0 : x.GetHashCode();

        public static readonly Func<Object, String> Show = x => x == null ? "" : x.ToString();

        public static A Id<A>(A val)
        {
            return val;
        }

        /// <summary>Curried versions of functions in the enclosing class.</summary>
        public static class C
        {
            public static readonly Func<bool, Func<bool, bool>> And = x => y => x & y;
            
            public static readonly Func<bool, Func<bool, bool>> Or = x => y => x | y;
            
            public static readonly Func<bool, Func<bool, bool>> Xor = x => y => x ^ y;
            
            public static readonly Func<int, Func<int, int>> Add = x => y => x + y;
            
            public static readonly Func<Object, Func<Object, bool>> Eq = x => y => Object.Equals(x, y);
            
            public static readonly Func<Object, Func<Object, bool>> Same = x => y => Object.ReferenceEquals(x, y);
        }
    }
}
