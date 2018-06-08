using System;
using System.Collections.Concurrent;

namespace KitchenSink
{
    public static partial class Operators
    {
        /// <summary>
        /// Forward function composition.
        /// </summary>
        public static Func<A, C> Compose<A, B, C>(
            Func<A, B> f,
            Func<B, C> g) =>
            x => g(f(x));

        /// <summary>
        /// Forward function composition.
        /// </summary>
        public static Func<A, D> Compose<A, B, C, D>(
            Func<A, B> f,
            Func<B, C> g,
            Func<C, D> h) =>
            x => h(g(f(x)));

        /// <summary>
        /// Forward function composition.
        /// </summary>
        public static Func<A, E> Compose<A, B, C, D, E>(
            Func<A, B> f,
            Func<B, C> g,
            Func<C, D> h,
            Func<D, E> i) =>
            x => i(h(g(f(x))));

        /// <summary>
        /// Forward function composition.
        /// </summary>
        public static Func<A, F> Compose<A, B, C, D, E, F>(
            Func<A, B> f,
            Func<B, C> g,
            Func<C, D> h,
            Func<D, E> i,
            Func<E, F> j) =>
            x => j(i(h(g(f(x)))));

        /// <summary>
        /// Function currying.
        /// </summary>
        public static Func<A, Func<B, C>> Curry<A, B, C>(
            Func<A, B, C> f) =>
            x => y => f(x, y);

        /// <summary>
        /// Function currying.
        /// </summary>
        public static Func<A, Func<B, Func<C, D>>> Curry<A, B, C, D>(
            Func<A, B, C, D> f) =>
            x => y => z => f(x, y, z);

        /// <summary>
        /// Function currying.
        /// </summary>
        public static Func<A, Func<B, Func<C, Func<D, E>>>> Curry<A, B, C, D, E>(
            Func<A, B, C, D, E> f) =>
            x => y => z => w => f(x, y, z, w);

        /// <summary>
        /// Function un-currying.
        /// </summary>
        public static Func<A, B, C> Uncurry<A, B, C>(
            Func<A, Func<B, C>> f) =>
            (x, y) => f(x)(y);

        /// <summary>
        /// Function un-currying.
        /// </summary>
        public static Func<A, B, C, D> Uncurry<A, B, C, D>(
            Func<A, Func<B, Func<C, D>>> f) =>
            (x, y, z) => f(x)(y)(z);

        /// <summary>
        /// Function un-currying.
        /// </summary>
        public static Func<A, B, C, D, E> Uncurry<A, B, C, D, E>(
            Func<A, Func<B, Func<C, Func<D, E>>>> f) =>
            (x, y, z, w) => f(x)(y)(z)(w);

        /// <summary>
        /// Partially apply 2-parameter function to 1 argument.
        /// </summary>
        public static Func<B, Z> Apply<A, B, Z>(Func<A, B, Z> f, A a) =>
            b => f(a, b);

        /// <summary>
        /// Partially apply 3-parameter function to 1 argument.
        /// </summary>
        public static Func<B, C, Z> Apply<A, B, C, Z>(Func<A, B, C, Z> f, A a) =>
            (b, c) => f(a, b, c);

        /// <summary>
        /// Partially apply 3-parameter function to 2 arguments.
        /// </summary>
        public static Func<C, Z> Apply<A, B, C, Z>(Func<A, B, C, Z> f, A a, B b) =>
            c => f(a, b, c);

        /// <summary>
        /// Partially apply 4-parameter function to 1 argument.
        /// </summary>
        public static Func<B, C, D, Z> Apply<A, B, C, D, Z>(Func<A, B, C, D, Z> f, A a) =>
            (b, c, d) => f(a, b, c, d);

        /// <summary>
        /// Partially apply 4-parameter function to 2 arguments.
        /// </summary>
        public static Func<C, D, Z> Apply<A, B, C, D, Z>(Func<A, B, C, D, Z> f, A a, B b) =>
            (c, d) => f(a, b, c, d);

        /// <summary>
        /// Partially apply 4-parameter function to 3 arguments.
        /// </summary>
        public static Func<D, Z> Apply<A, B, C, D, Z>(Func<A, B, C, D, Z> f, A a, B b, C c) =>
            d => f(a, b, c, d);

        /// <summary>
        /// Flip function arguments.
        /// </summary>
        public static Func<B, A, Z> Flip<A, B, Z>(Func<A, B, Z> f) =>
            (b, a) => f(a, b);

        /// <summary>
        /// Rotate function arguments forward.
        /// </summary>
        public static Func<C, A, B, Z> Rotate<A, B, C, Z>(Func<A, B, C, Z> f) =>
            (c, a, b) => f(a, b, c);

        /// <summary>
        /// Rotate function arguments backward.
        /// </summary>
        public static Func<B, C, A, Z> RotateBack<A, B, C, Z>(Func<A, B, C, Z> f) =>
            (b, c, a) => f(a, b, c);

        /// <summary>
        /// Wrap function with memoizing cache.
        /// </summary>
        public static Func<A, Z> Memo<A, Z>(Func<A, Z> f)
        {
            var hash = new ConcurrentDictionary<A, Z>();
            return a => hash.GetOrAdd(a, f);
        }

        /// <summary>
        /// Wrap function with memoizing cache.
        /// </summary>
        public static Func<A, B, Z> Memo<A, B, Z>(Func<A, B, Z> f)
        {
            var hash = new ConcurrentDictionary<(A, B), Z>();
            return (a, b) => hash.GetOrAdd((a, b), t => f(t.Item1, t.Item2));
        }

        /// <summary>
        /// Wrap function with memoizing cache.
        /// </summary>
        public static Func<A, B, C, Z> Memo<A, B, C, Z>(Func<A, B, C, Z> f)
        {
            var hash = new ConcurrentDictionary<(A, B, C), Z>();
            return (a, b, c) => hash.GetOrAdd((a, b, c), t => f(t.Item1, t.Item2, t.Item3));
        }

        /// <summary>
        /// Wrap function with memoizing cache.
        /// </summary>
        public static Func<A, B, C, D, Z> Memo<A, B, C, D, Z>(Func<A, B, C, D, Z> f)
        {
            var hash = new ConcurrentDictionary<(A, B, C, D), Z>();
            return (a, b, c, d) => hash.GetOrAdd((a, b, c, d), t => f(t.Item1, t.Item2, t.Item3, t.Item4));
        }
    }
}
