using System;
using static KitchenSink.Operators;

namespace KitchenSink.Extensions
{
    /// <summary>
    /// Extension methods on functions.
    /// </summary>
    public static class FuncExtensions
    {
        /// <summary>
        /// Partially applies 2-parameter function to 1 argument.
        /// </summary>
        public static Func<B, Z> Invoke<A, B, Z>(this Func<A, B, Z> f, A a)
        {
            return Apply(f, a);
        }

        /// <summary>
        /// Partially applies 3-parameter function to 1 argument.
        /// </summary>
        public static Func<B, C, Z> Invoke<A, B, C, Z>(this Func<A, B, C, Z> f, A a)
        {
            return Apply(f, a);
        }

        /// <summary>
        /// Partially applies 3-parameter function to 2 arguments.
        /// </summary>
        public static Func<C, Z> Invoke<A, B, C, Z>(this Func<A, B, C, Z> f, A a, B b)
        {
            return Apply(f, a, b);
        }

        /// <summary>
        /// Partially applies 4-parameter function to 1 argument.
        /// </summary>
        public static Func<B, C, D, Z> Invoke<A, B, C, D, Z>(this Func<A, B, C, D, Z> f, A a)
        {
            return Apply(f, a);
        }

        /// <summary>
        /// Partially applies 4-parameter function to 2 arguments.
        /// </summary>
        public static Func<C, D, Z> Invoke<A, B, C, D, Z>(this Func<A, B, C, D, Z> f, A a, B b)
        {
            return Apply(f, a, b);
        }

        /// <summary>
        /// Partially applies 4-parameter function to 3 arguments.
        /// </summary>
        public static Func<D, Z> Invoke<A, B, C, D, Z>(this Func<A, B, C, D, Z> f, A a, B b, C c)
        {
            return Apply(f, a, b, c);
        }

        /// <summary>
        /// Combines the results of two functions on the same input type.
        /// </summary>
        public static Func<A, D> Zip<A, B, C, D>(this Func<A, B> f, Func<A, C> g, Func<B, C, D> zipper)
        {
            return x => zipper(f(x), g(x));
        }

        /// <summary>
        /// Combines the results of two functions.
        /// </summary>
        public static Func<A, C, E> Join<A, B, C, D, E>(this Func<A, B> f, Func<C, D> g, Func<B, D, E> joiner)
        {
            return (x, y) => joiner(f(x), g(y));
        }

        /// <summary>
        /// Combines the results of a function applied to two arguments of the same type.
        /// </summary>
        public static Func<A, A, C> On<A, B, C>(this Func<B, B, C> combine, Func<A, B> selector)
        {
            return (x, y) => combine(selector(x), selector(y));
        }

        /// <summary>
        /// Reverse function composition.
        /// </summary>
        public static Func<A, C> Then<A, B, C>(this Func<A, B> f, Func<B, C> g)
        {
            return g.Compose(f);
        }

        /// <summary>
        /// Function composition.
        /// </summary>
        public static Func<A, C> Compose<A, B, C>(this Func<B, C> f, Func<A, B> g)
        {
            return x => f(g(x));
        }

        /// <summary>
        /// Curries 2-parameter function.
        /// </summary>
        public static Func<A, Func<B, C>> Curry<A, B, C>(this Func<A, B, C> f)
        {
            return a => b => f(a, b);
        }

        /// <summary>
        /// Curries 3-parameter function.
        /// </summary>
        public static Func<A, Func<B, Func<C, D>>> Curry<A, B, C, D>(this Func<A, B, C, D> f)
        {
            return a => b => c => f(a, b, c);
        }

        /// <summary>
        /// Curries 4-parameter function.
        /// </summary>
        public static Func<A, Func<B, Func<C, Func<D, E>>>> Curry<A, B, C, D, E>(this Func<A, B, C, D, E> f)
        {
            return a => b => c => d => f(a, b, c, d);
        }
    }
}
