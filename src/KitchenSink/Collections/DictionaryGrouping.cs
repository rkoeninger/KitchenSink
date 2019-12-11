using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KitchenSink.Collections
{
    public class DictionaryGrouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private readonly KeyValuePair<TKey, IEnumerable<TElement>> pair;

        public DictionaryGrouping(KeyValuePair<TKey, IEnumerable<TElement>> pair) => this.pair = pair;

        public TKey Key => pair.Key;

        public IEnumerator<TElement> GetEnumerator() => pair.Value.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
