using System;

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
        public static Func<B, Z> Apply<A, B, Z>(Func<A, B, Z> f, A a)
            => b => f.Invoke(a, b);

        /// <summary>
        /// Partially apply 3-parameter function to 1 argument.
        /// </summary>
        public static Func<B, C, Z> Apply<A, B, C, Z>(Func<A, B, C, Z> f, A a)
            => (b, c) => f.Invoke(a, b, c);

        /// <summary>
        /// Partially apply 3-parameter function to 2 arguments.
        /// </summary>
        public static Func<C, Z> Apply<A, B, C, Z>(Func<A, B, C, Z> f, A a, B b)
            => c => f.Invoke(a, b, c);

        /// <summary>
        /// Partially apply 4-parameter function to 1 argument.
        /// </summary>
        public static Func<B, C, D, Z> Apply<A, B, C, D, Z>(Func<A, B, C, D, Z> f, A a)
            => (b, c, d) => f.Invoke(a, b, c, d);

        /// <summary>
        /// Partially apply 4-parameter function to 2 arguments.
        /// </summary>
        public static Func<C, D, Z> Apply<A, B, C, D, Z>(Func<A, B, C, D, Z> f, A a, B b)
            => (c, d) => f.Invoke(a, b, c, d);

        /// <summary>
        /// Partially apply 4-parameter function to 3 arguments.
        /// </summary>
        public static Func<D, Z> Apply<A, B, C, D, Z>(Func<A, B, C, D, Z> f, A a, B b, C c)
            => d => f.Invoke(a, b, c, d);
    }
}
