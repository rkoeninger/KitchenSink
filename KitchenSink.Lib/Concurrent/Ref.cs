using System;

namespace KitchenSink.Concurrent
{
    /// <summary>
    /// Internal base class.
    /// </summary>
    public class Ref
    {
        internal bool pending;
        internal object value;
        internal object previous;

        internal Ref(object initial) => value = initial;

        internal void Commit()
        {
            pending = false;
            previous = default;
        }

        internal void Rollback()
        {
            pending = false;
            value = previous;
        }
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
        public Ref(Stm stm, A initial) : base(initial)
        {
            this.stm = stm;
        }

        /// <summary>
        /// Gets current committed value, regardless of pending value.
        /// Setting is done in single-statement transaction.
        /// </summary>
        public A Value
        {
            get => (A) value; // TODO: return previous value when accessor is outside current Tran
            set => stm.InTran(() => Update(_ => value));
        }

        /// <summary>
        /// Prepare update to contained value in ambient scope.
        /// </summary>
        public void Update(Func<A, A> f) => Update(Scope.Get<Tran>(), f);

        /// <summary>
        /// Prepare update to contained value in given scope.
        /// </summary>
        public void Update(Tran tran, Func<A, A> f)
        {
            if (tran == null)
            {
                throw new OutsideTranScopeException();
            }

            if (!pending)
            {
                pending = true;
                tran.Join(this);
                previous = value;
            }

            value = f((A) value);
        }
    }
}
