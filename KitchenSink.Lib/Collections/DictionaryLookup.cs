using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KitchenSink.Collections
{
    public class DictionaryLookup<TKey, TElement> : ILookup<TKey, TElement>
    {
        private readonly IDictionary<TKey, IEnumerable<TElement>> _dictionary;

        public DictionaryLookup(IDictionary<TKey, IEnumerable<TElement>> dictionary)
        {
            _dictionary = dictionary;
        }

        public IEnumerable<TElement> this[TKey key] => _dictionary[key];

        public int Count => _dictionary.Count;

        public bool Contains(TKey key) => _dictionary.ContainsKey(key);

        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
        {
            return _dictionary
                .Select(pair => new DictionaryGrouping<TKey, TElement>(pair))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
