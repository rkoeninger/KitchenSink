using System;

namespace KitchenSink.Concurrent
{
    /// <summary>
    /// Atomic value cell with synchronous updates.
    /// </summary>
    public static class Atom
    {
        /// <summary>
        /// Create new Atom with given initial value.
        /// </summary>
        public static Atom<A> Of<A>(A initial) => new Atom<A>(initial);
    }

    /// <summary>
    /// Atomic value cell with synchronous updates.
    /// </summary>
    public class Atom<A>
    {
        private A value;
        private readonly Lock @lock = new Lock();

        /// <summary>
        /// Create new Atom with given initial value.
        /// </summary>
        public Atom(A initial)
        {
            value = initial;
        }

        /// <summary>
        /// Synchronously set or get contained value.
        /// </summary>
        public A Value
        {
            get => value;
            set => @lock.Do(() => this.value = value);
        }

        /// <summary>
        /// Synchronously apply atomic transformation to contained value.
        /// </summary>
        public A Update(Func<A, A> f) => @lock.Do(() => value = f(value));
    }
}
