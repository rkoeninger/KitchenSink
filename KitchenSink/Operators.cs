using System;
using System.Collections;
using KitchenSink.Control;
using KitchenSink.Extensions;
using static KitchenSink.Comparison;

namespace KitchenSink
{
    /// <summary>
    /// Simple functions in a simple form.
    /// Suggested use of this class is with <c>using static</c>.
    /// </summary>
    public static partial class Operators
    {
        /// <summary>Logical negation.</summary>
        public static readonly Func<bool, bool> Not = x => !x;

        /// <summary>Logical implication.</summary>
        public static readonly Func<bool, bool, bool> Implies = (x, y) => !x || y;

        /// <summary>Composes predicate with logical negation.</summary>
        public static Func<A, bool> Complement<A>(Func<A, bool> f) => x => !f(x);

        /// <summary>Attempts parse of string to int.</summary>
        public static Func<string, Maybe<int>> ToInt = x => x.ToInt();

        /// <summary>Attempts parse of string to double.</summary>
        public static Func<string, Maybe<double>> ToDouble = x => x.ToDouble();

        /// <summary>Positive integer predicate.</summary>
        public static readonly Func<int, bool> Pos = x => x > 0;

        /// <summary>Non-positive integer predicate.</summary>
        public static readonly Func<int, bool> NonPos = x => x <= 0;

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
        public static readonly Func<IEnumerable, bool> Empty = x => !NonEmpty(x);

        /// <summary>Check if collection is non-empty.</summary>
        public static readonly Func<IEnumerable, bool> NonEmpty = x =>
        {
            var e = x.GetEnumerator();
            var disposable = e as IDisposable;

            if (disposable != null)
            {
                using (disposable)
                {
                    return e.MoveNext();
                }
            }

            return e.MoveNext();
        };

        /// <summary>Check if string is only whitespace.</summary>
        public static readonly Func<string, bool> Blank = x => string.IsNullOrWhiteSpace(x);

        /// <summary>Check if string is not only whitespace.</summary>
        public static readonly Func<string, bool> NonBlank = x => !Blank(x);

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

        /// <summary>Partially apply 2-parameter function to 1 argument.</summary>
        public static Func<B, Z> Apply<A, B, Z>(Func<A, B, Z> f, A a)
            => b => f.Invoke(a, b);

        /// <summary>Partially apply 3-parameter function to 1 argument.</summary>
        public static Func<B, C, Z> Apply<A, B, C, Z>(Func<A, B, C, Z> f, A a)
            => (b, c) => f.Invoke(a, b, c);

        /// <summary>Partially apply 3-parameter function to 2 arguments.</summary>
        public static Func<C, Z> Apply<A, B, C, Z>(Func<A, B, C, Z> f, A a, B b)
            => c => f.Invoke(a, b, c);

        /// <summary>Partially apply 4-parameter function to 1 argument.</summary>
        public static Func<B, C, D, Z> Apply<A, B, C, D, Z>(Func<A, B, C, D, Z> f, A a)
            => (b, c, d) => f.Invoke(a, b, c, d);

        /// <summary>Partially apply 4-parameter function to 2 arguments.</summary>
        public static Func<C, D, Z> Apply<A, B, C, D, Z>(Func<A, B, C, D, Z> f, A a, B b)
            => (c, d) => f.Invoke(a, b, c, d);

        /// <summary>Partially apply 4-parameter function to 3 arguments.</summary>
        public static Func<D, Z> Apply<A, B, C, D, Z>(Func<A, B, C, D, Z> f, A a, B b, C c)
            => d => f.Invoke(a, b, c, d);

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

        /// <summary>Starting point for a range comparison, example: <c>0 &lt;= Cmp(x) &lt; 10</c></summary>
        public static RangeComparison.Initial<A> Cmp<A>(A value) where A : IComparable<A>
            => RangeComparison.New(value);

        /// <summary>
        /// Comparison that returns a symbolic result.
        /// Null values are always less than non-null values.
        /// </summary>
        public static Comparison Compare<A>(A x, A y) where A : IComparable<A> =>
            x == null && y == null ? EQ :
            x == null ? LT :
            y == null ? GT :
            Case(x.CompareTo(y))
                .When(Neg).Then(LT)
                .When(Pos).Then(GT)
                .Else(EQ);

        /// <summary>
        /// Starts a Cond with the given condition.
        /// </summary>
        public static ICondInitial If(Func<bool> condition)
            => Cond.If(condition);

        /// <summary>
        /// Starts a Cond with the given condition.
        /// </summary>
        public static ICondInitial If(bool condition)
            => Cond.If(condition);

        /// <summary>
        /// Starts a Cond with the given condition.
        /// </summary>
        public static ICondIf<TResult> If<TResult>(Func<bool> condition)
            => Cond<TResult>.If(condition);

        /// <summary>
        /// Starts a Cond with the given condition.
        /// </summary>
        public static ICondIf<TResult> If<TResult>(bool condition)
            => Cond<TResult>.If(condition);

        /// <summary>
        /// Starts a Case for the given key.
        /// </summary>
        public static ICaseInitialThen<A> Case<A>(A key)
            => Control.Case.Of(key);

        /// <summary>
        /// Starts a Case for the given key with explicit return type.
        /// </summary>
        public static ICaseThen<TKey, TResult> Case<TKey, TResult>(TKey key)
            => Control.Case<TKey, TResult>.Of(key);

        /// <summary>
        /// Allows a block of statements to be used as an expression.
        /// </summary>
        public static A Do<A>(Func<A> body) => body();
    }
}
