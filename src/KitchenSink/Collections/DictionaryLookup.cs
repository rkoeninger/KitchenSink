using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KitchenSink.Collections
{
    public class DictionaryLookup<TKey, TElement> : ILookup<TKey, TElement>
    {
        private readonly IDictionary<TKey, IEnumerable<TElement>> dictionary;

        public DictionaryLookup(IDictionary<TKey, IEnumerable<TElement>> dictionary) => this.dictionary = dictionary;

        public IEnumerable<TElement> this[TKey key] => dictionary[key];

        public int Count => dictionary.Count;

        public bool Contains(TKey key) => dictionary.ContainsKey(key);

        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator() => dictionary
            .Select(pair => new DictionaryGrouping<TKey, TElement>(pair))
            .GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
