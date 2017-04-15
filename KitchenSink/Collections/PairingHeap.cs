using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KitchenSink.Operators;

namespace KitchenSink.Collections
{
    // https://en.wikipedia.org/wiki/Pairing_heap
    public class PairingHeap<A> where A : IComparable<A>
    {
        private readonly Maybe<Tuple<A, ICollection<PairingHeap<A>>>> contents;

        public PairingHeap()
        {
            contents = None<Tuple<A, ICollection<PairingHeap<A>>>>();
        }

        public PairingHeap(A value, ICollection<PairingHeap<A>> heaps)
        {
            contents = Some(Tuple.Create(value, heaps));
        }

        public A FindMin() => FindMinMaybe().OrElseThrow("Heap is empty");
        public Maybe<A> FindMinMaybe() => contents.Select(x => x.Item1);


    }
}
