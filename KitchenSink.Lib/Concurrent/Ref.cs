using System;
using System.Collections.Generic;
using System.Linq;
using KitchenSink.Extensions;
using static KitchenSink.Operators;

namespace KitchenSink.Concurrent
{
    /// <summary>
    /// Internal base class.
    /// </summary>
    public abstract class Ref
    {
        internal object value;
        internal List<object> previous = new List<object>();
        internal Ref(object initial) => value = initial;
        internal void Commit() => previous.Pop();
        internal void Rollback() => value = previous.Pop();
    }

    /// <summary>
    /// Atomic value cell with synchronous, co-ordinated updates.
    /// </summary>
    public sealed class Ref<A> : Ref
    {
        private readonly Stm stm;

        /// <summary>
        /// Create new Ref with given initial value.
        /// </summary>
        internal Ref(Stm stm, A initial) : base(initial)
        {
            this.stm = stm;
        }

        /// <summary>
        /// TODO: document
        /// </summary>
        public A Value
        {
            //get => Get();
            //set => Set(value);
            get => Cast<A>(Scope.GetMaybe<Tran>().HasValue || previous.Count == 0 ? value : previous.Last());
            set => stm.InTran(() => Update(_ => value));
        }

        public A Get()
        {
            // TODO: get value in ambient tran or committed value
            return default;
        }

        public A Get(Tran tran)
        {
            // TODO: get current value in tran
            return default;
        }

        public A Set(A a)
        {
            // TODO: set value in ambient tran or single-statement tran
            return a;
        }

        public A Set(Tran tran, A a)
        {
            // TODO: set value for given tran, join if needed
            return a;
        }

        // TODO: each Tran needs to have its own current value

        /// <summary>
        /// Prepare update to contained value in ambient scope.
        /// </summary>
        public void Update(Func<A, A> f)
        {
        }

        //Update(Scope.GetMaybe<Tran>().OrElseThrow<OutsideTranScopeException>(), f);

        /// <summary>
        /// Prepare update to contained value in given scope.
        /// </summary>
        public void Update(Tran tran, Func<A, A> f)
        {
            if (tran == null)
            {
            //    throw new OutsideTranScopeException();
                return;
            }

            if (tran.Join(this))
            {
                previous.Push(value);
            }

            value = f((A) value);
        }
    }
}
