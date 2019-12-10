using System;
using System.Linq;
using static KitchenSink.Operators;

namespace KitchenSink.Collections
{
    public class PairingHeap<A> where A : IComparable<A>
    {
        private class Contents
        {
            public A Value { get; set; }
            public IConsList<PairingHeap<A>> Heaps { get; set; }
        }

        private readonly Maybe<Contents> contents;

        public PairingHeap()
        {
            contents = None<Contents>();
        }

        public PairingHeap(A value, IConsList<PairingHeap<A>> heaps)
        {
            contents = Some(new Contents
            {
                Value = value,
                Heaps = heaps
            });
        }

        public A FindMin() => FindMinMaybe().OrElseThrow("Heap is empty");
        public Maybe<A> FindMinMaybe() => contents.Select(x => x.Value);

        public PairingHeap<A> Merge(PairingHeap<A> that) =>
            contents.Branch(
                mine =>
                    that.contents.Branch(
                        theirs =>
                            mine.Value.CompareTo(theirs.Value) < 0
                                ? new PairingHeap<A>(mine.Value, mine.Heaps.Cons(that))
                                : new PairingHeap<A>(theirs.Value, theirs.Heaps.Cons(this)),
                        () => this),
                () => that);

        public PairingHeap<A> Insert(A value) => Merge(new PairingHeap<A>(value, ConsList.Empty<PairingHeap<A>>()));

        public PairingHeap<A> DeleteMin() =>
            contents.OrElseThrow("Heap is empty")
                .Heaps
                .Aggregate(new PairingHeap<A>(), (acc, x) => acc.Merge(x));
    }
}
