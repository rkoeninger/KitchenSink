using System;
using static KitchenSink.Operators;

namespace KitchenSink.Extensions
{
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
    }
}
