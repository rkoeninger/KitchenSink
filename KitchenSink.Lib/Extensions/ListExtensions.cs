using System.Collections.Generic;
using System.Linq;

namespace KitchenSink.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Pop last item off of List, like with a stack.
        /// </summary>
        public static A Pop<A>(this List<A> list)
        {
            var last = Peek(list);
            list.RemoveAt(list.Count - 1);
            return last;
        }

        /// <summary>
        /// Push item onto end of List, like with a stack.
        /// </summary>
        public static void Push<A>(this List<A> list, A item) => list.Add(item);

        /// <summary>
        /// Peek at last item in List, like with a stack.
        /// </summary>
        public static A Peek<A>(this List<A> list) => list.Last();
    }
}
