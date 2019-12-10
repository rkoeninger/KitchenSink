using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KitchenSink.Collections
{
    public class DictionaryGrouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private readonly KeyValuePair<TKey, IEnumerable<TElement>> _pair;

        public DictionaryGrouping(KeyValuePair<TKey, IEnumerable<TElement>> pair) => _pair = pair;

        public TKey Key => _pair.Key;

        public IEnumerator<TElement> GetEnumerator() => _pair.Value.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
