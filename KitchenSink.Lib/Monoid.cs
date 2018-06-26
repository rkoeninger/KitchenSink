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
        /// Adds <c>int</c>s, with a default of <c>0</c>.
        /// </summary>
        public static readonly Monoid<int> IntSum =
            new DelegateMonoid<int>(Const(0), (x, y) => x + y);

        /// <summary>
        /// Multipies <c>int</c>s, with a default of <c>1</c>.
        /// </summary>
        public static readonly Monoid<int> IntProduct =
            new DelegateMonoid<int>(Const(1), (x, y) => x * y);

        /// <summary>
        /// Adds <c>long</c>s, with a default of <c>0</c>.
        /// </summary>
        public static readonly Monoid<long> LongSum =
            new DelegateMonoid<long>(Const(0L), (x, y) => x + y);

        /// <summary>
        /// Multipies <c>long</c>s, with a default of <c>1</c>.
        /// </summary>
        public static readonly Monoid<long> LongProduct =
            new DelegateMonoid<long>(Const(1L), (x, y) => x * y);

        /// <summary>
        /// Concats <c>String</c>s, with a default of <c>""</c>.
        /// </summary>
        public static Monoid<string> StringConcat =
            new DelegateMonoid<string>(Const(""), string.Concat);

        /// <summary>
        /// Concats <c>IEnumerable</c>s, with a default of <c>Empty</c>.
        /// </summary>
        public static Monoid<IEnumerable<A>> EnumerableConcat<A>() =>
            new DelegateMonoid<IEnumerable<A>>(Enumerable.Empty<A>, Enumerable.Concat);

        /// <summary>
        /// Concats <c>List</c>s, with a default of <c>Empty</c>.
        /// </summary>
        public static Monoid<List<A>> ListConcat<A>() =>
            new DelegateMonoid<List<A>>(() => ListOf<A>(), (x, y) => x.Concat(y).ToList());

        /// <summary>
        /// Concats <c>Unit</c>s, by simply ignoring them and
        /// returning <c>Unit</c>, with a default of <c>Unit</c>.
        /// </summary>
        public static Monoid<Unit> UnitIgnore =
            new DelegateMonoid<Unit>(Const(Unit.It), (_, __) => Unit.It);
    }

    /// <summary>
    /// Appends values of a particular type into
    /// a combined value of the same type.
    /// </summary>
    public abstract class Monoid<A>
    {
        /// <summary>
        /// Returns a default value for <c>A</c> for this operation.
        /// Also known as a "zero".
        /// </summary>
        public abstract A Default { get; }

        /// <summary>
        /// Appends the two argument values into a new, combined value.
        /// Also known as "add".
        /// </summary>
        public abstract A Append(A x, A y);

        /// <summary>
        /// Combines all values in given sequence into a single result.
        /// Also known as "concat"
        /// </summary>
        public A Aggregate(IEnumerable<A> seq) => seq.Aggregate(Default, Append);
    }

    internal class DelegateMonoid<A> : Monoid<A>
    {
        private readonly Func<A> zero;
        private readonly Func<A, A, A> append;

        internal DelegateMonoid(Func<A> zero, Func<A, A, A> append)
        {
            this.zero = zero;
            this.append = append;
        }

        public override A Default => zero();

        public override A Append(A x, A y) => append(x, y);
    }
}
