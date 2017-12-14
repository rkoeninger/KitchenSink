using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KitchenSink.Collections
{
    public class RoseTree<A> : IEnumerable<A>
    {
        public RoseTree(A value)
        {
            Value = value;
            Children = new List<RoseTree<A>>();
        }

        public RoseTree(A value, IEnumerable<RoseTree<A>> children)
        {
            Value = value;
            Children = children.ToList();
        }

        public A Value { get; }

        public IList<RoseTree<A>> Children { get; }

        public IEnumerator<A> GetEnumerator()
        {
            yield return Value;

            foreach (var child in Children)
            {
                foreach (var item in child)
                {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
