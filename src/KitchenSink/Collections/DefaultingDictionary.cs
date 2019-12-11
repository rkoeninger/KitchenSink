using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KitchenSink.Collections
{
    public class DefaultingDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        private IDictionary<TKey, TValue> Primary { get; }
        private IDictionary<TKey, TValue> Secondary { get; }

        public DefaultingDictionary(
            IDictionary<TKey, TValue> primary,
            IDictionary<TKey, TValue> secondary)
        {
            Primary = primary;
            Secondary = secondary;
        }

        public TValue this[TKey key]
        {
            get => Primary.TryGetValue(key, out var value) ? value : Secondary[key];
            set => Secondary[key] = value;
        }

        public ICollection<TKey> Keys
        {
            get
            {
                var keys = new HashSet<TKey>();
                keys.UnionWith(Primary.Keys);
                keys.UnionWith(Secondary.Keys);
                return keys;
            }
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        public ICollection<TValue> Values => Keys.Select(k => this[k]).ToArray();

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        public int Count => Keys.Count;

        public bool IsReadOnly => Primary.IsReadOnly;

        public void Add(KeyValuePair<TKey, TValue> item) => Primary.Add(item);

        public void Add(TKey key, TValue value) => Primary.Add(key, value);

        public bool Contains(KeyValuePair<TKey, TValue> item) =>
            Primary.Contains(item) || Secondary.Contains(item);

        public bool ContainsKey(TKey key) => Primary.ContainsKey(key) || Secondary.ContainsKey(key);

        public bool TryGetValue(TKey key, out TValue value) =>
            Primary.TryGetValue(key, out value) || Secondary.TryGetValue(key, out value);

        private IEnumerable<KeyValuePair<TKey, TValue>> Enumerate() =>
            Keys.Select(k => new KeyValuePair<TKey, TValue>(k, this[k]));

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => Enumerate().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var pair in this)
            {
                array[arrayIndex++] = pair;
            }
        }

        private static T RemovalError<T>() =>
            throw new NotSupportedException("Removal operations not supported on DefaultingDictionary");

        public bool Remove(KeyValuePair<TKey, TValue> item) => RemovalError<bool>();

        public bool Remove(TKey key) => RemovalError<bool>();

        public void Clear() => RemovalError<Void>();
    }
}
