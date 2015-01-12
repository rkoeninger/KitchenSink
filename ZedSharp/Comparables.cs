using System;
using System.Runtime.CompilerServices;

namespace ZedSharp
{
    public static class Comparables
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLessThan<A>(this A val, A that) where A : IComparable<A>
        {
            return val.CompareTo(that) < 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLessThanOrEqualTo<A>(this A val, A that) where A : IComparable<A>
        {
            return val.CompareTo(that) <= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGreaterThan<A>(this A val, A that) where A : IComparable<A>
        {
            return val.CompareTo(that) > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGreaterThanOrEqualTo<A>(this A val, A that) where A : IComparable<A>
        {
            return val.CompareTo(that) >= 0;
        }

        /// <summary>Inclusive on lower bound, exclusive on upper bound.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBetween<A>(this A val, A lower, A upper) where A : IComparable<A>
        {
            return val.IsGreaterThanOrEqualTo(lower) && val.IsLessThan(upper);
        }

        /// <summary>Inclusive on lower bound, exclusive on upper bound.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotBetween<A>(this A val, A lower, A upper) where A : IComparable<A>
        {
            return ! val.IsBetween(lower, upper);
        }
    }
}
