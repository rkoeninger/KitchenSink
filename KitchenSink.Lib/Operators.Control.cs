using System;
using KitchenSink.Control;

namespace KitchenSink
{
    public static partial class Operators
    {
        /// <summary>
        /// Let binding.
        /// </summary>
        public static Z Let<A, Z>(A a, Func<A, Z> body) => body(a);

        /// <summary>
        /// Let binding.
        /// </summary>
        public static Z Let<A, B, Z>(A a, B b, Func<A, B, Z> body) => body(a, b);

        /// <summary>
        /// Let binding.
        /// </summary>
        public static Z Let<A, B, C, Z>(A a, B b, C c, Func<A, B, C, Z> body) => body(a, b, c);

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
        public static ICaseInitialThen<A> Switch<A>(A key)
            => Case.Of(key);

        /// <summary>
        /// Starts a Case for the given key with explicit return type.
        /// </summary>
        public static ICaseThen<TKey, TResult> Switch<TKey, TResult>(TKey key)
            => Case<TKey, TResult>.Of(key);

        /// <summary>
        /// Allows a block of statements to be used as an expression.
        /// </summary>
        public static A Do<A>(Func<A> body) => body();
    }
}
