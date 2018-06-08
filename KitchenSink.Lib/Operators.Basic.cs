using System;
using KitchenSink.Extensions;

namespace KitchenSink
{
    /// <summary>
    /// Simple functions in a simple form.
    /// Suggested use of this class is with <c>using static</c>.
    /// </summary>
    public static partial class Operators
    {
        /// <summary>
        /// Logical negation.
        /// </summary>
        public static readonly Func<bool, bool> Not = x => !x;

        /// <summary>
        /// Logical implication.
        /// </summary>
        public static readonly Func<bool, bool, bool> Implies = (x, y) => !x || y;

        /// <summary>
        /// Composes predicate with logical negation.
        /// </summary>
        public static Func<A, bool> Complement<A>(Func<A, bool> f) => x => !f(x);

        /// <summary>
        /// Attempts parse of string to int.
        /// </summary>
        public static readonly Func<string, Maybe<int>> ToInt = x => x.ToInt();

        /// <summary>
        /// Attempts parse of string to double.
        /// </summary>
        public static readonly Func<string, Maybe<double>> ToDouble = x => x.ToDouble();

        /// <summary>
        /// Attempts parse of string to enum.
        /// </summary>
        public static Maybe<A> ToEnum<A>(string x) where A : struct => x.ToEnum<A>();

        /// <summary>
        /// Positive integer predicate.
        /// </summary>
        public static readonly Func<int, bool> Pos = x => x > 0;

        /// <summary>
        /// Non-positive integer predicate.
        /// </summary>
        public static readonly Func<int, bool> NonPos = x => x <= 0;

        /// <summary>
        /// Negative integer predicate.
        /// </summary>
        public static readonly Func<int, bool> Neg = x => x < 0;

        /// <summary>
        /// Non-negative integer predicate.
        /// </summary>
        public static readonly Func<int, bool> NonNeg = x => x >= 0;

        /// <summary>
        /// Even integer predicate.
        /// </summary>
        public static readonly Func<int, bool> Even = x => x % 2 == 0;

        /// <summary>
        /// Odd integer predicate.
        /// </summary>
        public static readonly Func<int, bool> Odd = x => x % 2 != 0;

        /// <summary>
        /// Integer addition.
        /// </summary>
        public static readonly Func<int, int, int> Add = (x, y) => x + y;

        /// <summary>
        /// Integer negation.
        /// </summary>
        public static readonly Func<int, int> Negate = x => -x;

        /// <summary>
        /// Integer increment.
        /// </summary>
        public static readonly Func<int, int> Inc = x => x + 1;

        /// <summary>
        /// Integer decrement.
        /// </summary>
        public static readonly Func<int, int> Dec = x => x - 1;

        /// <summary>
        /// Bitwise mask.
        /// </summary>
        public static Func<byte, byte> Mask(byte x) => y => (byte)(x & y);

        /// <summary>
        /// Bitwise mask.
        /// </summary>
        public static Func<sbyte, sbyte> Mask(sbyte x) => y => (sbyte)(x & y);

        /// <summary>
        /// Bitwise mask.
        /// </summary>
        public static Func<short, short> Mask(short x) => y => (short)(x & y);

        /// <summary>
        /// Bitwise mask.
        /// </summary>
        public static Func<ushort, ushort> Mask(ushort x) => y => (ushort)(x & y);

        /// <summary>
        /// Bitwise mask.
        /// </summary>
        public static Func<int, int> Mask(int x) => y => x & y;

        /// <summary>
        /// Bitwise mask.
        /// </summary>
        public static Func<uint, uint> Mask(uint x) => y => x & y;

        /// <summary>
        /// Bitwise mask.
        /// </summary>
        public static Func<long, long> Mask(long x) => y => x & y;

        /// <summary>
        /// Bitwise mask.
        /// </summary>
        public static Func<ulong, ulong> Mask(ulong x) => y => x & y;

        /// <summary>
        /// Get object's type.
        /// </summary>
        public static readonly Func<object, Type> TypeOf = x => x.GetType();

        /// <summary>
        /// Object equality.
        /// </summary>
        public static bool Eq<A>(A x, A y) => Equals(x, y);

        /// <summary>
        /// Object reference equality - same object.
        /// </summary>
        public static readonly Func<object, object, bool> Same = ReferenceEquals;

        /// <summary>
        /// Null check.
        /// </summary>
        public static readonly Func<object, bool> Null = x => x == null;

        /// <summary>
        /// Non-null check.
        /// </summary>
        public static readonly Func<object, bool> NonNull = x => x != null;

        /// <summary>
        /// Get hash code for object.
        /// </summary>
        public static readonly Func<object, int> Hash = x => x?.GetHashCode() ?? 0;

        /// <summary>
        /// Get string for object.
        /// </summary>
        public static readonly Func<object, string> Str = x => x?.ToString() ?? "";

        /// <summary>
        /// Check if string is only whitespace.
        /// </summary>
        public static readonly Func<string, bool> Blank = string.IsNullOrWhiteSpace;

        /// <summary>
        /// Check if string is not only whitespace.
        /// </summary>
        public static readonly Func<string, bool> NonBlank = x => !Blank(x);

        /// <summary>
        /// Identity function.
        /// </summary>
        public static A Id<A>(A val) => val;

        /// <summary>
        /// Constant function.
        /// </summary>
        public static Func<A> Const<A>(A val) => () => val;

        /// <summary>
        /// Inline dynamic cast.
        /// </summary>
        public static dynamic Dyn(dynamic val) => val;

        /// <summary>
        /// Performs cast/conversion to type parameter.
        /// </summary>
        public static A Cast<A>(object val) => (A)val;

        /// <summary>
        /// Attempts cast/conversion to type parameter.
        /// </summary>
        public static Maybe<A> CastMaybe<A>(object val)
            => val is A ? Some(Cast<A>(val)) : None<A>();

        /// <summary>
        /// Performs cast to type parameter.
        /// </summary>
        public static A As<A>(object val) where A : class => val as A;

        /// <summary>
        /// Type check for type parameter.
        /// </summary>
        public static bool Is<A>(object val) => val is A;

        /// <summary>
        /// Type check for type parameter.
        /// </summary>
        public static Func<A, bool> Is<A, B>() => val => val is B;

        /// <summary>
        /// Negative type check for type parameter.
        /// </summary>
        public static bool IsNot<A>(object val) => !Is<A>(val);
    }
}
