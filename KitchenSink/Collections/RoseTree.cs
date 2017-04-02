using System.Collections.Generic;
using System.Linq;

namespace KitchenSink.Collections
{
    public class RoseTree<A>
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
    }
}
