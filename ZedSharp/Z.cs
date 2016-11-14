using System;
using System.Collections;

namespace ZedSharp
{
    /// <summary>A collection of often useful, first-class functions.</summary>
    public static class Z
    {
        /// <summary>Logical negation.</summary>
        public static readonly Func<bool, bool> Not = x => !x;

        /// <summary>Eager logical and.</summary>
        public static readonly Func<bool, bool, bool> And = (x, y) => x & y;

        /// <summary>Eager logical or.</summary>
        public static readonly Func<bool, bool, bool> Or = (x, y) => x | y;

        /// <summary>Logical exclusive-or.</summary>
        public static readonly Func<bool, bool, bool> Xor = (x, y) => x ^ y;

        /// <summary>Positive integer predicate.</summary>
        public static readonly Func<int, bool> Pos = x => x > 0;

        /// <summary>Negative integer predicate.</summary>
        public static readonly Func<int, bool> Neg = x => x < 0;

        /// <summary>Non-negative integer predicate.</summary>
        public static readonly Func<int, bool> NotNeg = x => x >= 0;

        /// <summary>Even integer predicate.</summary>
        public static readonly Func<int, bool> Even = x => x % 2 == 0;

        /// <summary>Odd integer predicate.</summary>
        public static readonly Func<int, bool> Odd = x => x % 2 != 0;

        /// <summary>Integer addition.</summary>
        public static readonly Func<int, int, int> Add = (x, y) => x + y;
        
        /// <summary>Integer negation.</summary>
        public static readonly Func<int, int> Negate = x => -x;

        /// <summary>Integer increment.</summary>
        public static readonly Func<int, int> Inc = x => x + 1;

        /// <summary>Integer decrement.</summary>
        public static readonly Func<int, int> Dec = x => x - 1;

        /// <summary>Get object's type.</summary>
        public static readonly Func<object, Type> Type = x => x.GetType();

        /// <summary>Object equality.</summary>
        public static readonly Func<object, object, bool> Eq = Equals;

        /// <summary>Object reference equality - same object.</summary>
        public static readonly Func<object, object, bool> Same = ReferenceEquals;

        /// <summary>Get hash code for object.</summary>
        public static readonly Func<object, int> Hash = x => x == null ? 0 : x.GetHashCode();

        /// <summary>Get string for object.</summary>
        public static readonly Func<object, string> Str = x => x == null ? "" : x.ToString();

        /// <summary>Check if collection is empty.</summary>
        public static readonly Func<ICollection, bool> Empty = x => x.Count == 0;

        /// <summary>Check if collection is non-empty.</summary>
        public static readonly Func<ICollection, bool> NonEmpty = x => x.Count > 0;
        
        /// <summary>Identity function.</summary>
        public static A Id<A>(A val)
        {
            return val;
        }

        /// <summary>Inline dynamic cast.</summary>
        public static dynamic Dyn(dynamic val)
        {
            return val;
        }

        /// <summary>Performs cast/conversion to type parameter.</summary>
        public static A Cast<A>(object val)
        {
            return (A) val;
        }

        /// <summary>Performs cast to type parameter.</summary>
        public static A As<A>(object val) where A : class
        {
            return val as A;
        }

        /// <summary>Type check for type parameter.</summary>
        public static bool Is<A>(object val)
        {
            return val is A;
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

        /// <summary>Curried generic comparison function.</summary>
        public static Func<A, int> Compare<A>(A x) where A : IComparable<A>
        {
            return y => x.CompareTo(y);
        }
    }
}
