using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using KitchenSink.Collections;

namespace KitchenSink
{
    public static partial class Operators
    {
        /// <summary>
        /// Infinitely enumerates items returned from provided function.
        /// Example: <c>f => [f(), f(), f(), ...]</c>
        /// </summary>
        public static IEnumerable<A> Forever<A>(Func<A> f)
        {
            while (true)
            {
                yield return f();
            }

            // ReSharper disable once IteratorNeverReturns
        }

        /// <summary>
        /// Generates a sequence based on given function.
        /// Function should return None to indicate end of sequence.
        /// </summary>
        public static IEnumerable<A> Separate<A>(Func<Maybe<A>> f)
        {
            var (value, hasValue) = f();

            while (hasValue)
            {
                yield return value;
                (value, hasValue) = f();
            }
        }

        /// <summary>
        /// Generates a sequence based on given function.
        /// Function should return None to indicate end of sequence.
        /// </summary>
        public static IEnumerable<A> Separate<A>(A initial, Func<A, Maybe<A>> f)
        {
            var (value, hasValue) = f(initial);

            while (hasValue)
            {
                yield return value;
                (value, hasValue) = f(value);
            }
        }

        /// <summary>
        /// Repeatedly calls function against previous return value
        /// so long as true is also returned. Final result is returned
        /// by this method.
        /// </summary>
        public static A Recur<A>(A initial, Func<A, (bool, A)> f)
        {
            while (true)
            {
                var (resume, x) = f(initial);

                if (!resume)
                {
                    return x;
                }

                initial = x;
            }
        }

        /// <summary>
        /// Returns sequence of same value given number of times.
        /// Example: <c>Repeat(5, 'a') => ['a', 'a', 'a', 'a', 'a']</c>
        /// </summary>
        public static IEnumerable<A> Repeat<A>(int count, A value) => Repeatedly(count, Const(value));

        /// <summary>
        /// Returns sequence populated by calling function given number of times.
        /// Example: <c>Repeatedly(5, Rand) => [2354, 7456, 9623, 3764, 6475]</c>
        /// </summary>
        public static IEnumerable<A> Repeatedly<A>(int count, Func<A> f) => Forever(f).Take(count);

        /// <summary>
        /// Returns infinite sequence counting up from the given starting value.
        /// </summary>
        public static IEnumerable<int> From(int start) => From(start, Inc);

        /// <summary>
        /// Returns infinite sequence counting up from the given starting value.
        /// </summary>
        public static IEnumerable<int> From(int start, int inc) => From(start, Curry(Add)(inc));

        /// <summary>
        /// Returns infinite sequence counting up from the given starting value.
        /// </summary>
        public static IEnumerable<A> From<A>(A start, Func<A, A> inc)
        {
            while (true)
            {
                yield return start;
                start = inc(start);
            }

            // ReSharper disable once IteratorNeverReturns
        }

        /// <summary>
        /// Returns sequences of values in specified enum type.
        /// </summary>
        public static IEnumerable<A> ValuesOf<A>() => typeof(A).GetEnumValues().OfType<A>();

        /// <summary>
        /// Creates an Either with the given Left value.
        /// </summary>
        public static Either<A, B> LeftOf<A, B>(A value) => new(true, value, default);

        /// <summary>
        /// Creates an Either with the given Right value.
        /// </summary>
        public static Either<A, B> RightOf<A, B>(B value) => new(false, default, value);

        /// <summary>
        /// Creates a Maybe that has the given value if it is not null.
        /// </summary>
        public static Maybe<A> MaybeOf<A>(A value) => new(value);

        /// <summary>
        /// Creates a Maybe with the given value.
        /// </summary>
        public static Maybe<A> Some<A>(A value) => new(value, true);

        /// <summary>
        /// Creates a Maybe without a value.
        /// </summary>
        public static Maybe<A> None<A>() => Maybe<A>.None;

        public static Maybe<Z> AllSome<A, Z>(
            Maybe<A> ma,
            Func<A, Z> selector) => ma.Select(selector);

        public static Maybe<Z> AllSome<A, B, Z>(
            Maybe<A> ma,
            Maybe<B> mb,
            Func<A, B, Z> selector) =>
            from a in ma
            from b in mb
            select selector(a, b);

        public static Maybe<Z> AllSome<A, B, C, Z>(
            Maybe<A> ma,
            Maybe<B> mb,
            Maybe<C> mc,
            Func<A, B, C, Z> selector) =>
            from a in ma
            from b in mb
            from c in mc
            select selector(a, b, c);

        public static Maybe<Z> AllSome<A, B, C, D, Z>(
            Maybe<A> ma,
            Maybe<B> mb,
            Maybe<C> mc,
            Maybe<D> md,
            Func<A, B, C, D, Z> selector) =>
            from a in ma
            from b in mb
            from c in mc
            from d in md
            select selector(a, b, c, d);

        public static Maybe<Z> AllSome<A, B, C, D, E, Z>(
            Maybe<A> ma,
            Maybe<B> mb,
            Maybe<C> mc,
            Maybe<D> md,
            Maybe<E> me,
            Func<A, B, C, D, E, Z> selector) =>
            from a in ma
            from b in mb
            from c in mc
            from d in md
            from e in me
            select selector(a, b, c, d, e);

        public static Maybe<Z> AllSome<A, B, C, D, E, F, Z>(
            Maybe<A> ma,
            Maybe<B> mb,
            Maybe<C> mc,
            Maybe<D> md,
            Maybe<E> me,
            Maybe<F> mf,
            Func<A, B, C, D, E, F, Z> selector) =>
            from a in ma
            from b in mb
            from c in mc
            from d in md
            from e in me
            from f in mf
            select selector(a, b, c, d, e, f);

        public static Maybe<Z> AllSome<A, B, C, D, E, F, G, Z>(
            Maybe<A> ma,
            Maybe<B> mb,
            Maybe<C> mc,
            Maybe<D> md,
            Maybe<E> me,
            Maybe<F> mf,
            Maybe<G> mg,
            Func<A, B, C, D, E, F, G, Z> selector) =>
            from a in ma
            from b in mb
            from c in mc
            from d in md
            from e in me
            from f in mf
            from g in mg
            select selector(a, b, c, d, e, f, g);

        public static Maybe<Z> AllSome<A, B, C, D, E, F, G, H, Z>(
            Maybe<A> ma,
            Maybe<B> mb,
            Maybe<C> mc,
            Maybe<D> md,
            Maybe<E> me,
            Maybe<F> mf,
            Maybe<G> mg,
            Maybe<H> mh,
            Func<A, B, C, D, E, F, G, H, Z> selector) =>
            from a in ma
            from b in mb
            from c in mc
            from d in md
            from e in me
            from f in mf
            from g in mg
            from h in mh
            select selector(a, b, c, d, e, f, g, h);

        public static Maybe<Z> AllSome<A, B, C, D, E, F, G, H, I, Z>(
            Maybe<A> ma,
            Maybe<B> mb,
            Maybe<C> mc,
            Maybe<D> md,
            Maybe<E> me,
            Maybe<F> mf,
            Maybe<G> mg,
            Maybe<H> mh,
            Maybe<I> mi,
            Func<A, B, C, D, E, F, G, H, I, Z> selector) =>
            from a in ma
            from b in mb
            from c in mc
            from d in md
            from e in me
            from f in mf
            from g in mg
            from h in mh
            from i in mi
            select selector(a, b, c, d, e, f, g, h, i);

        public static Maybe<Z> AllSome<A, B, C, D, E, F, G, H, I, J, Z>(
            Maybe<A> ma,
            Maybe<B> mb,
            Maybe<C> mc,
            Maybe<D> md,
            Maybe<E> me,
            Maybe<F> mf,
            Maybe<G> mg,
            Maybe<H> mh,
            Maybe<I> mi,
            Maybe<J> mj,
            Func<A, B, C, D, E, F, G, H, I, J, Z> selector) =>
            from a in ma
            from b in mb
            from c in mc
            from d in md
            from e in me
            from f in mf
            from g in mg
            from h in mh
            from i in mi
            from j in mj
            select selector(a, b, c, d, e, f, g, h, i, j);

        public static IEnumerable<A> SeqOf<A>(params A[] values) => values;

        public static A[] ArrayOf<A>(params A[] values) => values;

        public static List<A> ListOf<A>(params A[] values) => new(values);

        public static HashSet<A> SetOf<A>(params A[] values) => new(values);

        public static ConcurrentBag<A> BagOf<A>(params A[] values) => new(values);

        public static Queue<A> QueueOf<A>(params A[] values) => new(values);

        public static Stack<A> StackOf<A>(params A[] values) => new(values);

        public static Cons ConsOf(object car, object cdr) => new(car, cdr);

        public static Dictionary<A, V> DictOf<A, V>(params (A, V)[] pairs) => pairs.ToDictionary();

        public static (A, B) TupleOf<A, B>(A a, B b) =>
            (a, b);

        public static (A, B, C) TupleOf<A, B, C>(A a, B b, C c) =>
            (a, b, c);

        public static (A, B, C, D) TupleOf<A, B, C, D>(A a, B b, C c, D d) =>
            (a, b, c, d);

        public static (A, B, C, D, E) TupleOf<A, B, C, D, E>(A a, B b, C c, D d, E e) =>
            (a, b, c, d, e);

        public static (A, B, C, D, E, F) TupleOf<A, B, C, D, E, F>(A a, B b, C c, D d, E e, F f) =>
            (a, b, c, d, e, f);

        public static (A, B, C, D, E, F, G) TupleOf<A, B, C, D, E, F, G>(A a, B b, C c, D d, E e, F f, G g) =>
            (a, b, c, d, e, f, g);

        public static (A, B, C, D, E, F, G, H) TupleOf<A, B, C, D, E, F, G, H>(A a, B b, C c, D d, E e, F f, G g, H h) =>
            (a, b, c, d, e, f, g, h);

        /// <summary>
        /// Creates a new Dictionary from the properties of an object.
        /// </summary>
        /// <remarks>
        /// Intended to be used with an anonymous object, but can be used with any object.
        /// </remarks>
        public static Dictionary<string, object> ToDictionary(object obj) =>
            obj?
                .GetType()
                .GetProperties()
                .Where(x => x.GetIndexParameters().Length == 0)
                .ToDictionary(x => x.Name, x => x.GetValue(obj, null))
            ?? new Dictionary<string, object>();

        public static Dictionary<A, V> ToDictionary<A, V>(this (A, V)[] pairs) =>
            pairs.ToDictionary(x => x.Item1, x => x.Item2);
    }
}
