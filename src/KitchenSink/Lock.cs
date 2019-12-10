using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using KitchenSink.Control;
using KitchenSink.Extensions;
using static KitchenSink.Operators;

namespace KitchenSink
{
    public abstract class Lock
    {
        public static Lock New() => new SimpleLock();

        public static Lock Of(params Lock[] locks) => new MultiLock(locks);

        public abstract void Enter();
        public abstract void Exit();

        public A Do<A>(Func<A> f)
        {
            try
            {
                Enter();
                return f();
            }
            finally
            {
                Exit();
            }
        }

        public void Do(Action f) => Do(f.AsFunc());
    }

    internal sealed class SimpleLock : Lock
    {
        internal readonly Guid id = Guid.NewGuid();
        private readonly object handle = new object();

        public override void Enter() => Monitor.Enter(handle);

        public override void Exit()
        {
            if (Monitor.IsEntered(handle))
            {
                Monitor.Exit(handle);
            }
        }
    }

    internal sealed class MultiLock : Lock
    {
        private readonly SimpleLock[] locks;

        internal MultiLock(IEnumerable<Lock> locks)
        {
            this.locks = locks.Flatten(x =>
                Case.Of(x)
                    .When<SimpleLock>().Then(LeftOf<Lock, IEnumerable<Lock>>(x))
                    .When<MultiLock>().Then(m => (RightOf<Lock, IEnumerable<Lock>>(m.locks)))
                    .End()
                    .OrElseThrow<InvalidSubtypeException>())
                .Cast<SimpleLock>()
                .Distinct()
                .OrderBy(x => x.id)
                .ToArray();
        }

        public override void Enter() => locks.ForEach(x => x.Enter());
        public override void Exit() => locks.ForEach(x => x.Exit());
    }
}
