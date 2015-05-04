using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ZedSharp
{
    public class Dictionary<A, B, V> : IDictionary<Tuple<A, B>, V>, IReadOnlyDictionary<Tuple<A, B>, V>
    {
        private readonly IDictionary<Tuple<A, B>, V> inner = new Dictionary<Tuple<A, B>, V>();

        public IEnumerator<KeyValuePair<Tuple<A, B>, V>> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        public void Clear()
        {
            inner.Clear();
        }

        public void CopyTo(KeyValuePair<Tuple<A, B>, V>[] array, int arrayIndex)
        {
            inner.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return inner.Count; }
        }

        public bool IsReadOnly
        {
            get { return inner.IsReadOnly; }
        }

        public bool Contains(KeyValuePair<Tuple<A, B>, V> item)
        {
            return inner.Contains(item);
        }

        public bool ContainsKey(Tuple<A, B> key)
        {
            return inner.ContainsKey(key);
        }

        public bool ContainsKeys(A k1, B k2)
        {
            return ContainsKey(Tuple.Create(k1, k2));
        }

        public bool ContainsKey1(A k1)
        {
            // TODO this could be better implemented
            return Keys1.Contains(k1);
        }

        public bool ContainsKey2(B k2)
        {
            // TODO this could be better implemented
            return Keys2.Contains(k2);
        }

        public void Add(KeyValuePair<Tuple<A, B>, V> item)
        {
            inner.Add(item);
        }

        public void Add(Tuple<A, B> key, V value)
        {
            inner.Add(key, value);
        }

        public void Add(A k1, B k2, V value)
        {
            Add(Tuple.Create(k1, k2), value);
        }

        public bool Remove(KeyValuePair<Tuple<A, B>, V> item)
        {
            return inner.Remove(item);
        }

        public bool Remove(Tuple<A, B> key)
        {
            return inner.Remove(key);
        }

        public bool Remove(A k1, B k2)
        {
            return Remove(Tuple.Create(k1, k2));
        }

        public bool TryGetValue(Tuple<A, B> key, out V value)
        {
            return inner.TryGetValue(key, out value);
        }

        public bool TryGetValue(A k1, B k2, out V value)
        {
            return inner.TryGetValue(Tuple.Create(k1, k2), out value);
        }

        public V this[Tuple<A, B> key]
        {
            get { return inner[key]; }
            set { inner[key] = value; }
        }

        public V this[A k1, B k2]
        {
            get { return this[Tuple.Create(k1, k2)]; }
            set { this[Tuple.Create(k1, k2)] = value; }
        }

        public Maybe<V> GetMaybe(Tuple<A, B> key)
        {
            V result;
            return TryGetValue(key, out result) ? Maybe.Some(result) : Maybe<V>.None;
        }

        public Maybe<V> GetMaybe(A k1, B k2)
        {
            V result;
            return TryGetValue(Tuple.Create(k1, k2), out result) ? Maybe.Some(result) : Maybe<V>.None;
        }

        public ICollection<Tuple<A, B>> Keys
        {
            get { return inner.Keys; }
        }

        public ICollection<A> Keys1
        {
            get { return inner.Keys.Select(x => x.Item1).Distinct().ToArray(); }
        }

        public ICollection<B> Keys2
        {
            get { return inner.Keys.Select(x => x.Item2).Distinct().ToArray(); }
        }

        public ICollection<V> Values
        {
            get { return inner.Values; }
        }

        IEnumerable<Tuple<A, B>> IReadOnlyDictionary<Tuple<A, B>, V>.Keys
        {
            get { return Keys; }
        }

        IEnumerable<V> IReadOnlyDictionary<Tuple<A, B>, V>.Values
        {
            get { return Values; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Dictionary<A, B, C, V> : IDictionary<Tuple<A, B, C>, V>, IReadOnlyDictionary<Tuple<A, B, C>, V>
    {
        private readonly IDictionary<Tuple<A, B, C>, V> inner = new Dictionary<Tuple<A, B, C>, V>();

        public IEnumerator<KeyValuePair<Tuple<A, B, C>, V>> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        public void Clear()
        {
            inner.Clear();
        }

        public void CopyTo(KeyValuePair<Tuple<A, B, C>, V>[] array, int arrayIndex)
        {
            inner.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return inner.Count; }
        }

        public bool IsReadOnly
        {
            get { return inner.IsReadOnly; }
        }

        public bool Contains(KeyValuePair<Tuple<A, B, C>, V> item)
        {
            return inner.Contains(item);
        }

        public bool ContainsKey(Tuple<A, B, C> key)
        {
            return inner.ContainsKey(key);
        }

        public bool ContainsKeys(A k1, B k2, C k3)
        {
            return ContainsKey(Tuple.Create(k1, k2, k3));
        }

        public bool ContainsKey1(A k1)
        {
            // TODO this could be better implemented
            return Keys1.Contains(k1);
        }

        public bool ContainsKey2(B k2)
        {
            // TODO this could be better implemented
            return Keys2.Contains(k2);
        }

        public bool ContainsKey3(C k3)
        {
            // TODO this could be better implemented
            return Keys3.Contains(k3);
        }

        public void Add(KeyValuePair<Tuple<A, B, C>, V> item)
        {
            inner.Add(item);
        }

        public void Add(Tuple<A, B, C> key, V value)
        {
            inner.Add(key, value);
        }

        public void Add(A k1, B k2, C k3, V value)
        {
            Add(Tuple.Create(k1, k2, k3), value);
        }

        public bool Remove(KeyValuePair<Tuple<A, B, C>, V> item)
        {
            return inner.Remove(item);
        }

        public bool Remove(Tuple<A, B, C> key)
        {
            return inner.Remove(key);
        }

        public bool Remove(A k1, B k2, C k3)
        {
            return Remove(Tuple.Create(k1, k2, k3));
        }

        public bool TryGetValue(Tuple<A, B, C> key, out V value)
        {
            return inner.TryGetValue(key, out value);
        }

        public bool TryGetValue(A k1, B k2, C k3, out V value)
        {
            return inner.TryGetValue(Tuple.Create(k1, k2, k3), out value);
        }

        public V this[Tuple<A, B, C> key]
        {
            get { return inner[key]; }
            set { inner[key] = value; }
        }

        public V this[A k1, B k2, C k3]
        {
            get { return this[Tuple.Create(k1, k2, k3)]; }
            set { this[Tuple.Create(k1, k2, k3)] = value; }
        }

        public Maybe<V> GetMaybe(Tuple<A, B, C> key)
        {
            V result;
            return TryGetValue(key, out result) ? Maybe.Some(result) : Maybe<V>.None;
        }

        public Maybe<V> GetMaybe(A k1, B k2, C k3)
        {
            V result;
            return TryGetValue(Tuple.Create(k1, k2, k3), out result) ? Maybe.Some(result) : Maybe<V>.None;
        }

        public ICollection<Tuple<A, B, C>> Keys
        {
            get { return inner.Keys; }
        }

        public ICollection<A> Keys1
        {
            get { return inner.Keys.Select(x => x.Item1).Distinct().ToArray(); }
        }

        public ICollection<B> Keys2
        {
            get { return inner.Keys.Select(x => x.Item2).Distinct().ToArray(); }
        }

        public ICollection<C> Keys3
        {
            get { return inner.Keys.Select(x => x.Item3).Distinct().ToArray(); }
        }

        public ICollection<V> Values
        {
            get { return inner.Values; }
        }

        IEnumerable<Tuple<A, B, C>> IReadOnlyDictionary<Tuple<A, B, C>, V>.Keys
        {
            get { return Keys; }
        }

        IEnumerable<V> IReadOnlyDictionary<Tuple<A, B, C>, V>.Values
        {
            get { return Values; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
