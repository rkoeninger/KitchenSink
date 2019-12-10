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
        /// Check if collection is empty.
        /// </summary>
        public static bool Empty(IEnumerable x) => !NonEmpty(x);

        /// <summary>
        /// Check if collection is non-empty.
        /// </summary>
        public static bool NonEmpty(IEnumerable x)
        {
            if (x is ICollection c)
            {
                return c.Count > 0;
            }

            var e = x.GetEnumerator();

            if (e is IDisposable d)
            {
                using (d)
                {
                    return e.MoveNext();
                }
            }

            return e.MoveNext();
        }

        /// <summary>
        /// Check if collection contains exactly 1 element.
        /// </summary>
        public static bool One(IEnumerable x)
        {
            if (x is ICollection c)
            {
                return c.Count == 1;
            }

            var e = x.GetEnumerator();

            if (e is IDisposable d)
            {
                using (d)
                {
                    return e.MoveNext() && !e.MoveNext();
                }
            }

            return e.MoveNext() && !e.MoveNext();
        }

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
            var x = f();

            while (x.HasValue)
            {
                yield return x.Value;
                x = f();
            }
        }

        /// <summary>
        /// Generates a sequence based on given function.
        /// Function should return None to indicate end of sequence.
        /// </summary>
        public static IEnumerable<A> Separate<A>(A initial, Func<A, Maybe<A>> f)
        {
            var x = f(initial);

            while (x.HasValue)
            {
                initial = x.Value;
                yield return x.Value;
                x = f(initial);
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
        /// Adds up all numeric values in sequence.
        /// </summary>
        public static int Sum(IEnumerable<int> seq) => seq.Sum();

        /// <summary>
        /// Adds up all numeric values in sequence.
        /// </summary>
        public static long Sum(IEnumerable<long> seq) => seq.Sum();

        /// <summary>
        /// Creates an Either with the given Left value.
        /// </summary>
        public static Either<A, B> LeftOf<A, B>(A value) => new Either<A, B>(true, value, default);

        /// <summary>
        /// Creates an Either with the given Right value.
        /// </summary>
        public static Either<A, B> RightOf<A, B>(B value) => new Either<A, B>(false, default, value);

        /// <summary>
        /// Creates a Maybe that has the given value if it is not null.
        /// </summary>
        public static Maybe<A> MaybeOf<A>(A value) => new Maybe<A>(value);

        /// <summary>
        /// Creates a Maybe with the given value.
        /// </summary>
        public static Maybe<A> Some<A>(A value) => new Maybe<A>(value, true);

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

        public static List<A> ListOf<A>(params A[] values) => new List<A>(values);

        public static HashSet<A> SetOf<A>(params A[] values) => new HashSet<A>(values);

        public static ConcurrentBag<A> BagOf<A>(params A[] values) => new ConcurrentBag<A>(values);

        public static Queue<A> QueueOf<A>(params A[] values) => new Queue<A>(values);

        public static Cons ConsOf(object car, object cdr) => new Cons(car, cdr);

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

        public static Dictionary<A, V> DictOf<A, V>(params (A, V)[] pairs) => pairs.ToDictionary();

        public static Dictionary<A, V> DictOf<A, V>() => new Dictionary<A, V>();

        public static Dictionary<A, V> DictOf<A, V>(
            A k0, V v0) => new Dictionary<A, V>
        {
            {k0, v0}
        };

        public static Dictionary<A, V> DictOf<A, V>(
            A k0, V v0,
            A k1, V v1) => new Dictionary<A, V>
        {
            {k0, v0},
            {k1, v1}
        };

        public static Dictionary<A, V> DictOf<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2) => new Dictionary<A, V>
        {
            {k0, v0},
            {k1, v1},
            {k2, v2}
        };

        public static Dictionary<A, V> DictOf<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3) => new Dictionary<A, V>
        {
            {k0, v0},
            {k1, v1},
            {k2, v2},
            {k3, v3}
        };

        public static Dictionary<A, V> DictOf<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4) => new Dictionary<A, V>
        {
            {k0, v0},
            {k1, v1},
            {k2, v2},
            {k3, v3},
            {k4, v4}
        };

        public static Dictionary<A, V> DictOf<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5) => new Dictionary<A, V>
        {
            {k0, v0},
            {k1, v1},
            {k2, v2},
            {k3, v3},
            {k4, v4},
            {k5, v5}
        };

        public static Dictionary<A, V> DictOf<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6) => new Dictionary<A, V>
        {
            {k0, v0},
            {k1, v1},
            {k2, v2},
            {k3, v3},
            {k4, v4},
            {k5, v5},
            {k6, v6}
        };

        public static Dictionary<A, V> DictOf<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6,
            A k7, V v7) => new Dictionary<A, V>
        {
            {k0, v0},
            {k1, v1},
            {k2, v2},
            {k3, v3},
            {k4, v4},
            {k5, v5},
            {k6, v6},
            {k7, v7}
        };

        public static Dictionary<A, V> DictOf<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6,
            A k7, V v7,
            A k8, V v8) => new Dictionary<A, V>
        {
            {k0, v0},
            {k1, v1},
            {k2, v2},
            {k3, v3},
            {k4, v4},
            {k5, v5},
            {k6, v6},
            {k7, v7},
            {k8, v8}
        };

        public static Dictionary<A, V> DictOf<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6,
            A k7, V v7,
            A k8, V v8,
            A k9, V v9) => new Dictionary<A, V>
        {
            {k0, v0},
            {k1, v1},
            {k2, v2},
            {k3, v3},
            {k4, v4},
            {k5, v5},
            {k6, v6},
            {k7, v7},
            {k8, v8},
            {k9, v9}
        };

        public static Dictionary<A, V> DictOf<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6,
            A k7, V v7,
            A k8, V v8,
            A k9, V v9,
            A k10, V v10) => new Dictionary<A, V>
        {
            {k0, v0},
            {k1, v1},
            {k2, v2},
            {k3, v3},
            {k4, v4},
            {k5, v5},
            {k6, v6},
            {k7, v7},
            {k8, v8},
            {k9, v9},
            {k10, v10}
        };

        public static Dictionary<A, V> DictOf<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6,
            A k7, V v7,
            A k8, V v8,
            A k9, V v9,
            A k10, V v10,
            A k11, V v11) => new Dictionary<A, V>
        {
            {k0, v0},
            {k1, v1},
            {k2, v2},
            {k3, v3},
            {k4, v4},
            {k5, v5},
            {k6, v6},
            {k7, v7},
            {k8, v8},
            {k9, v9},
            {k10, v10},
            {k11, v11}
        };

        public static Dictionary<A, V> DictOf<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6,
            A k7, V v7,
            A k8, V v8,
            A k9, V v9,
            A k10, V v10,
            A k11, V v11,
            A k12, V v12) => new Dictionary<A, V>
        {
            {k0, v0},
            {k1, v1},
            {k2, v2},
            {k3, v3},
            {k4, v4},
            {k5, v5},
            {k6, v6},
            {k7, v7},
            {k8, v8},
            {k9, v9},
            {k10, v10},
            {k11, v11},
            {k12, v12}
        };

        public static Dictionary<A, V> DictOf<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6,
            A k7, V v7,
            A k8, V v8,
            A k9, V v9,
            A k10, V v10,
            A k11, V v11,
            A k12, V v12,
            A k13, V v13) => new Dictionary<A, V>
        {
            {k0, v0},
            {k1, v1},
            {k2, v2},
            {k3, v3},
            {k4, v4},
            {k5, v5},
            {k6, v6},
            {k7, v7},
            {k8, v8},
            {k9, v9},
            {k10, v10},
            {k11, v11},
            {k12, v12},
            {k13, v13}
        };

        public static Dictionary<A, V> DictOf<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6,
            A k7, V v7,
            A k8, V v8,
            A k9, V v9,
            A k10, V v10,
            A k11, V v11,
            A k12, V v12,
            A k13, V v13,
            A k14, V v14) => new Dictionary<A, V>
        {
            {k0, v0},
            {k1, v1},
            {k2, v2},
            {k3, v3},
            {k4, v4},
            {k5, v5},
            {k6, v6},
            {k7, v7},
            {k8, v8},
            {k9, v9},
            {k10, v10},
            {k11, v11},
            {k12, v12},
            {k13, v13},
            {k14, v14}
        };

        public static Dictionary<A, V> DictOf<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6,
            A k7, V v7,
            A k8, V v8,
            A k9, V v9,
            A k10, V v10,
            A k11, V v11,
            A k12, V v12,
            A k13, V v13,
            A k14, V v14,
            A k15, V v15) => new Dictionary<A, V>
        {
            {k0, v0},
            {k1, v1},
            {k2, v2},
            {k3, v3},
            {k4, v4},
            {k5, v5},
            {k6, v6},
            {k7, v7},
            {k8, v8},
            {k9, v9},
            {k10, v10},
            {k11, v11},
            {k12, v12},
            {k13, v13},
            {k14, v14},
            {k15, v15}
        };

        public static Dictionary<A, V> DictOf<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6,
            A k7, V v7,
            A k8, V v8,
            A k9, V v9,
            A k10, V v10,
            A k11, V v11,
            A k12, V v12,
            A k13, V v13,
            A k14, V v14,
            A k15, V v15,
            A k16, V v16) => new Dictionary<A, V>
        {
            {k0, v0},
            {k1, v1},
            {k2, v2},
            {k3, v3},
            {k4, v4},
            {k5, v5},
            {k6, v6},
            {k7, v7},
            {k8, v8},
            {k9, v9},
            {k10, v10},
            {k11, v11},
            {k12, v12},
            {k13, v13},
            {k14, v14},
            {k15, v15},
            {k16, v16}
        };

        public static Dictionary<A, V> DictOf<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6,
            A k7, V v7,
            A k8, V v8,
            A k9, V v9,
            A k10, V v10,
            A k11, V v11,
            A k12, V v12,
            A k13, V v13,
            A k14, V v14,
            A k15, V v15,
            A k16, V v16,
            A k17, V v17) => new Dictionary<A, V>
        {
            {k0, v0},
            {k1, v1},
            {k2, v2},
            {k3, v3},
            {k4, v4},
            {k5, v5},
            {k6, v6},
            {k7, v7},
            {k8, v8},
            {k9, v9},
            {k10, v10},
            {k11, v11},
            {k12, v12},
            {k13, v13},
            {k14, v14},
            {k15, v15},
            {k16, v16},
            {k17, v17}
        };

        public static Dictionary<A, V> DictOf<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6,
            A k7, V v7,
            A k8, V v8,
            A k9, V v9,
            A k10, V v10,
            A k11, V v11,
            A k12, V v12,
            A k13, V v13,
            A k14, V v14,
            A k15, V v15,
            A k16, V v16,
            A k17, V v17,
            A k18, V v18) => new Dictionary<A, V>
        {
            {k0, v0},
            {k1, v1},
            {k2, v2},
            {k3, v3},
            {k4, v4},
            {k5, v5},
            {k6, v6},
            {k7, v7},
            {k8, v8},
            {k9, v9},
            {k10, v10},
            {k11, v11},
            {k12, v12},
            {k13, v13},
            {k14, v14},
            {k15, v15},
            {k16, v16},
            {k17, v17},
            {k18, v18}
        };

        public static Dictionary<A, V> DictOf<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6,
            A k7, V v7,
            A k8, V v8,
            A k9, V v9,
            A k10, V v10,
            A k11, V v11,
            A k12, V v12,
            A k13, V v13,
            A k14, V v14,
            A k15, V v15,
            A k16, V v16,
            A k17, V v17,
            A k18, V v18,
            A k19, V v19) => new Dictionary<A, V>
        {
            {k0, v0},
            {k1, v1},
            {k2, v2},
            {k3, v3},
            {k4, v4},
            {k5, v5},
            {k6, v6},
            {k7, v7},
            {k8, v8},
            {k9, v9},
            {k10, v10},
            {k11, v11},
            {k12, v12},
            {k13, v13},
            {k14, v14},
            {k15, v15},
            {k16, v16},
            {k17, v17},
            {k18, v18},
            {k19, v19}
        };
    }
}
