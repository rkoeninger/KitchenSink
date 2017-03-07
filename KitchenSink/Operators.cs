using System;
using System.Collections;
using KitchenSink.Control;

namespace KitchenSink
{
    /// <summary>
    /// Simple functions in a simple form.
    /// Suggested use of this class is with <c>using static</c>.
    /// </summary>
    public static class Operators
    {
        /// <summary>Logical negation.</summary>
        public static readonly Func<bool, bool> Not = x => !x;

        /// <summary>Logical implication.</summary>
        public static readonly Func<bool, bool, bool> Implies = (x, y) => !x || y;

        /// <summary>Positive integer predicate.</summary>
        public static readonly Func<int, bool> Pos = x => x > 0;

        /// <summary>Negative integer predicate.</summary>
        public static readonly Func<int, bool> Neg = x => x < 0;

        /// <summary>Non-negative integer predicate.</summary>
        public static readonly Func<int, bool> NonNeg = x => x >= 0;

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
        public static readonly Func<object, Type> TypeOf = x => x.GetType();

        /// <summary>Object equality.</summary>
        public static bool Eq<A>(A x, A y) => Equals(x, y);

        /// <summary>Object reference equality - same object.</summary>
        public static readonly Func<object, object, bool> Same = ReferenceEquals;

        /// <summary>Null check.</summary>
        public static readonly Func<object, bool> Null = x => x == null;

        /// <summary>Non-null check.</summary>
        public static readonly Func<object, bool> NonNull = x => x != null;

        /// <summary>Get hash code for object.</summary>
        public static readonly Func<object, int> Hash = x => x?.GetHashCode() ?? 0;

        /// <summary>Get string for object.</summary>
        public static readonly Func<object, string> Str = x => x?.ToString() ?? "";

        /// <summary>Check if collection is empty.</summary>
        public static readonly Func<ICollection, bool> Empty = x => x.Count == 0;

        /// <summary>Check if collection is non-empty.</summary>
        public static readonly Func<ICollection, bool> NonEmpty = x => x.Count > 0;

        /// <summary>Identity function.</summary>
        public static A Id<A>(A val) => val;

        /// <summary>Inline dynamic cast.</summary>
        public static dynamic Dyn(dynamic val) => val;

        /// <summary>Performs cast/conversion to type parameter.</summary>
        public static A Cast<A>(object val) => (A)val;

        /// <summary>Performs cast to type parameter.</summary>
        public static A As<A>(object val) where A : class => val as A;

        /// <summary>Type check for type parameter.</summary>
        public static bool Is<A>(object val) => val is A;

        /// <summary>Function composition.</summary>
        public static Func<A, C> Compose<A, B, C>(Func<B, C> f, Func<A, B> g) => x => f(g(x));

        /// <summary>Function currying</summary>
        public static Func<A, Func<B, C>> Curry<A, B, C>(Func<A, B, C> f) => x => y => f(x, y);

        /// <summary>Curried comparision.</summary>
        public static Func<A, bool> LowerBound<A>(A x) where A : IComparable<A>
            => y => y.CompareTo(x) >= 0;

        /// <summary>Curried comparision.</summary>
        public static Func<A, bool> UpperBound<A>(A x) where A : IComparable<A>
            => y => y.CompareTo(x) <= 0;

        /// <summary>Curried comparision.</summary>
        public static Func<A, bool> ExclusiveLowerBound<A>(A x) where A : IComparable<A>
            => y => y.CompareTo(x) > 0;

        /// <summary>Curried comparision.</summary>
        public static Func<A, bool> ExclusiveUpperBound<A>(A x) where A : IComparable<A>
            => y => y.CompareTo(x) < 0;

        // TODO: this needs to be reconsidered
        /// <summary>Starting point for a range comparison: <c>Z.Cmp - 0 &lt;= x &lt; 10</c></summary>
        public static readonly RangeComparison0 Cmp = new RangeComparison0();

        /// <summary>
        /// Comparison that returns a symbolic result.
        /// Null values are always less than non-null values.
        /// </summary>
        public static Comparison Compare<A>(A x, A y) where A : IComparable<A>
        {
            if (x == null && y == null)
                return Comparison.EQ;
            if (x == null)
                return Comparison.LT;
            if (y == null)
                return Comparison.GT;
            return Case(x.CompareTo(y))
                .When(Neg).Then(Comparison.LT)
                .When(Pos).Then(Comparison.GT)
                .Else(Comparison.EQ);
        }

        /// <summary>
        /// Starts a Cond with the given condition.
        /// </summary>
        public static ICondInitial If(Func<bool> condition)
        {
            return Cond.If(condition);
        }

        /// <summary>
        /// Starts a Cond with the given condition.
        /// </summary>
        public static ICondInitial If(bool condition)
        {
            return Cond.If(condition);
        }

        /// <summary>
        /// Starts a Cond with the given condition.
        /// </summary>
        public static ICondIf<TResult> If<TResult>(Func<bool> condition)
        {
            return Cond<TResult>.If(condition);
        }

        /// <summary>
        /// Starts a Cond with the given condition.
        /// </summary>
        public static ICondIf<TResult> If<TResult>(bool condition)
        {
            return Cond<TResult>.If(condition);
        }

        /// <summary>
        /// Starts a Case for the given key.
        /// </summary>
        public static ICaseInitialThen<A> Case<A>(A key)
        {
            return Control.Case.Of(key);
        }

        /// <summary>
        /// Starts a Case for the given key with explicit return type.
        /// </summary>
        public static ICaseThen<TKey, TResult> Case<TKey, TResult>(TKey key)
        {
            return Control.Case<TKey, TResult>.Of(key);
        }

        /// <summary>
        /// Allows a block of statements to be used as an expression.
        /// </summary>
        public static A Do<A>(Func<A> body)
        {
            return body();
        }
    }
}
