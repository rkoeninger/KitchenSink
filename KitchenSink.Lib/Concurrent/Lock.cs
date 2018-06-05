using System;

namespace KitchenSink.Concurrent
{
    /// <summary>
    /// A mechanism to co-ordinate operations using basic CLR <c>lock</c>.
    /// </summary>
    public class Lock
    {
        private readonly object handle = new object();

        /// <summary>
        /// Invokes function without any overlapping invocations.
        /// </summary>
        public A Do<A>(Func<A> f)
        {
            lock (handle)
            {
                return f();
            }
        }

        /// <summary>
        /// Invokes function without any overlapping invocations.
        /// </summary>
        public void Do(Action f)
        {
            lock (handle)
            {
                f();
            }
        }
    }
}
