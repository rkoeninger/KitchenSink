using KitchenSink.Extensions;
using static KitchenSink.Operators;

namespace KitchenSink.Collections
{
    public static class BankersQueue
    {
        public static BankersQueue<A> Empty<A>() => new BankersQueue<A>();
    }

    public class BankersQueue<A>
    {
        private readonly IConsList<A> incoming;
        private readonly IConsList<A> outgoing;

        internal BankersQueue() : this(ConsList.Empty<A>(), ConsList.Empty<A>())
        {
        }

        private BankersQueue(IConsList<A> incoming, IConsList<A> outgoing)
        {
            this.incoming = incoming;
            this.outgoing = outgoing;
        }

        public BankersQueue<A> Enqueue(A value) => new BankersQueue<A>(incoming.Cons(value), outgoing);

        public Maybe<A> Current => outgoing.HeadMaybe.Or(incoming.LastMaybe());

        public (BankersQueue<A>, Maybe<A>) Dequeue()
        {
            if (outgoing.IsEmpty)
            {
                if (incoming.IsEmpty)
                {
                    return (this, None<A>());
                }

                var newIncoming = incoming.InReverse();
                return (new BankersQueue<A>(ConsList.Empty<A>(), newIncoming.Tail), newIncoming.Head);
            }

            return (new BankersQueue<A>(incoming, outgoing.Tail), outgoing.Head);
        }
    }
}
