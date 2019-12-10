using System;
using System.Collections.Generic;
using System.Linq;
using static KitchenSink.Operators;

namespace KitchenSink
{
    /// <summary>
    /// Appends values of a particular type into
    /// a combined value of the same type.
    /// </summary>
    public static class Monoid
    {
        /// <summary>
        /// Creates a new Monoid from the given functions.
        /// </summary>
        public static Monoid<A> Of<A>(Func<A> zero, Func<A, A, A> append) =>
            new Monoid<A>(zero, append);

        /// <summary>
        /// And's <see cref="bool"/>s, with a default of <c>true</c>.
        /// </summary>
        public static readonly Monoid<bool> BoolAnd =
            Of(Const(true), (x, y) => x & y);

        /// <summary>
        /// Or's <see cref="bool"/>s, with a default of <c>false</c>.
        /// </summary>
        public static readonly Monoid<bool> BoolOr =
            Of(Const(false), (x, y) => x | y);

        /// <summary>
        /// Adds <see cref="int"/>s, with a default of <c>0</c>.
        /// </summary>
        public static readonly Monoid<int> IntSum =
            Of(Const(0), (x, y) => x + y);

        /// <summary>
        /// Multipies <see cref="int"/>s, with a default of <c>1</c>.
        /// </summary>
        public static readonly Monoid<int> IntProduct =
            Of(Const(1), (x, y) => x * y);

        /// <summary>
        /// Adds <see cref="long"/>s, with a default of <c>0</c>.
        /// </summary>
        public static readonly Monoid<long> LongSum =
            Of(Const(0L), (x, y) => x + y);

        /// <summary>
        /// Multipies <see cref="long"/>s, with a default of <c>1</c>.
        /// </summary>
        public static readonly Monoid<long> LongProduct =
            Of(Const(1L), (x, y) => x * y);

        /// <summary>
        /// Concats <see cref="string"/>s, with a default of <c>""</c>.
        /// </summary>
        public static Monoid<string> StringConcat =
            Of(Const(""), string.Concat);

        /// <summary>
        /// Concats <see cref="IEnumerable{A}"/>s, with a default of <c>Empty</c>.
        /// </summary>
        public static Monoid<IEnumerable<A>> EnumerableConcat<A>() =>
            Of(Enumerable.Empty<A>, Enumerable.Concat);

        /// <summary>
        /// Concats <see cref="List{A}"/>s, with a default of <c>Empty</c>.
        /// </summary>
        public static Monoid<List<A>> ListConcat<A>() =>
            Of(() => ListOf<A>(), (x, y) => x.Concat(y).ToList());

        /// <summary>
        /// Concats <see cref="Unit"/>s, by simply ignoring them and
        /// returning <c>Unit</c>, with a default of <c>Unit</c>.
        /// </summary>
        public static Monoid<Unit> UnitIgnore =
            Of(Const(Unit.It), (_, __) => Unit.It);

        /// <summary>
        /// Combines <c>Some</c>s, ignoring <c>None</c>s.
        /// </summary>
        public static Monoid<Maybe<A>> Maybe<A>(Monoid<A> monoid) =>
            Of(None<A>, (x, y) => x.AndOr(y, monoid.Concat));

        /// <summary>
        /// Combines <see cref="IO{A}"/>s but constructing new <c>IO</c>s
        /// that eval and append the results of the original <c>IO</c>s.
        /// </summary>
        public static Monoid<IO<A>> IO<A>(Monoid<A> monoid) => Of(
            () => KitchenSink.IO.Of(() => monoid.Default),
            (x, y) => KitchenSink.IO.Of(monoid.Concat(x.Eval(), y.Eval())));

        /// <summary>
        /// Composes endomorphic functions left to right.
        /// </summary>
        public static Monoid<Func<A, A>> Func<A>() => Of<Func<A, A>>(() => Id, Compose);
    }

    /// <summary>
    /// Appends values of a particular type into
    /// a combined value of the same type.
    /// </summary>
    public class Monoid<A>
    {
        private readonly Func<A> zero;
        private readonly Func<A, A, A> append;

        public Monoid(Func<A> zero, Func<A, A, A> append)
        {
            this.zero = zero;
            this.append = append;
        }

        /// <summary>
        /// Returns a default value for <c>A</c> for this operation.
        /// </summary>
        public A Default => zero();

        /// <summary>
        /// Appends the two argument values into a new, combined value.
        /// </summary>
        public A Concat(A x, A y) => append(x, y);

        /// <summary>
        /// Combines all values in given sequence into a single result.
        /// </summary>
        public A Aggregate(IEnumerable<A> seq) => seq.Aggregate(Default, Concat);
    }
}
