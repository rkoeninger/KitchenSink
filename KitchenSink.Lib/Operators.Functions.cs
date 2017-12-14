using System;

namespace KitchenSink
{
    public static partial class Operators
    {
        /// <summary>
        /// Function composition.
        /// </summary>
        public static Func<A, C> Compose<A, B, C>(Func<B, C> f, Func<A, B> g) => x => f(g(x));

        /// <summary>
        /// Function currying.
        /// </summary>
        public static Func<A, Func<B, C>> Curry<A, B, C>(Func<A, B, C> f) => x => y => f(x, y);

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
