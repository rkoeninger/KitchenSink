using System;

namespace KitchenSink.Concurrent
{
    public class Lock
    {
        private readonly object handle = new object();

        public A Do<A>(Func<A> f)
        {
            lock (handle)
            {
                return f();
            }
        }

        public void Do(Action f)
        {
            lock (handle)
            {
                f();
            }
        }
    }

    public class Lock<A>
    {
        private readonly Lock @lock = new Lock();
        private A val;

        public Lock()
        {
        }

        public Lock(A val)
        {
            this.val = val;
        }

        public A Value
        {
            get => @lock.Do(() => val);
            set => @lock.Do(() => { val = value; });
        }

        public A Do(Func<A, A> f) => @lock.Do(() => f(val));

        public void Do(Action<A> f) => @lock.Do(() => f(val));

        public A Map(Func<A, A> f) => @lock.Do(() => val = f(val));
    }
}
