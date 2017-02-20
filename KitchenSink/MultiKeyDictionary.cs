using System;
using System.Collections.Generic;
using System.Linq;

namespace KitchenSink
{
    /// <summary>
    /// A 2-key Dictionary.
    /// </summary>
    public class Dictionary<TKey1, TKey2, TValue> : Dictionary<Tuple<TKey1, TKey2>, TValue>
    {
        public Dictionary() { }

        public Dictionary(IEqualityComparer<Tuple<TKey1, TKey2>> comparer) : base(comparer) { }

        public bool ContainsKeys(TKey1 a, TKey2 b)
        {
            return ContainsKey(Tuple.Create(a, b));
        }

        public bool ContainsKey1(TKey1 a)
        {
            return Keys.Any(x => Equals(x.Item1, a));
        }

        public bool ContainsKey2(TKey2 b)
        {
            return Keys.Any(x => Equals(x.Item2, b));
        }

        public ICollection<TKey1> Keys1 => Keys.Select(x => x.Item1).Distinct().ToList();
        public ICollection<TKey2> Keys2 => Keys.Select(x => x.Item2).Distinct().ToList();

        public void Add(TKey1 a, TKey2 b, TValue value)
        {
            Add(Tuple.Create(a, b), value);
        }

        public bool Remove(TKey1 a, TKey2 b)
        {
            return Remove(Tuple.Create(a, b));
        }

        public bool TryGetValue(TKey1 a, TKey2 b, out TValue value)
        {
            return TryGetValue(Tuple.Create(a, b), out value);
        }

        public TValue this[TKey1 a, TKey2 b]
        {
            get { return this[Tuple.Create(a, b)]; }
            set { this[Tuple.Create(a, b)] = value; }
        }

        public Maybe<TValue> GetMaybe(TKey1 a, TKey2 b)
        {
            TValue result;
            return TryGetValue(Tuple.Create(a, b), out result)
                ? Maybe.Some(result)
                : Maybe<TValue>.None;
        }
    }

    /// <summary>
    /// A 3-key dictionary.
    /// </summary>
    public class Dictionary<TKey1, TKey2, TKey3, TValue> : Dictionary<Tuple<TKey1, TKey2, TKey3>, TValue>
    {
        public Dictionary() { }

        public Dictionary(IEqualityComparer<Tuple<TKey1, TKey2, TKey3>> comparer) : base(comparer) { }

        public bool ContainsKeys(TKey1 a, TKey2 b, TKey3 c)
        {
            return ContainsKey(Tuple.Create(a, b, c));
        }

        public bool ContainsKey1(TKey1 a)
        {
            return Keys.Any(x => Equals(x.Item1, a));
        }

        public bool ContainsKey2(TKey2 b)
        {
            return Keys.Any(x => Equals(x.Item2, b));
        }

        public bool ContainsKey3(TKey3 c)
        {
            return Keys.Any(x => Equals(x.Item3, c));
        }

        public ICollection<TKey1> Keys1 => Keys.Select(x => x.Item1).Distinct().ToList();
        public ICollection<TKey2> Keys2 => Keys.Select(x => x.Item2).Distinct().ToList();
        public ICollection<TKey3> Keys3 => Keys.Select(x => x.Item3).Distinct().ToList();

        public void Add(TKey1 a, TKey2 b, TKey3 c, TValue value)
        {
            Add(Tuple.Create(a, b, c), value);
        }

        public bool Remove(TKey1 a, TKey2 b, TKey3 c)
        {
            return Remove(Tuple.Create(a, b, c));
        }

        public bool TryGetValue(TKey1 a, TKey2 b, TKey3 c, out TValue value)
        {
            return TryGetValue(Tuple.Create(a, b, c), out value);
        }

        public TValue this[TKey1 a, TKey2 b, TKey3 c]
        {
            get { return this[Tuple.Create(a, b, c)]; }
            set { this[Tuple.Create(a, b, c)] = value; }
        }

        public Maybe<TValue> GetMaybe(TKey1 a, TKey2 b, TKey3 c)
        {
            TValue result;
            return TryGetValue(Tuple.Create(a, b, c), out result)
                ? Maybe.Some(result)
                : Maybe<TValue>.None;
        }
    }

    /// <summary>
    /// A 4-key dictionary.
    /// </summary>
    public class Dictionary<TKey1, TKey2, TKey3, TKey4, TValue> : Dictionary<Tuple<TKey1, TKey2, TKey3, TKey4>, TValue>
    {
        public Dictionary() { }

        public Dictionary(IEqualityComparer<Tuple<TKey1, TKey2, TKey3, TKey4>> comparer) : base(comparer) { }

        public bool ContainsKeys(TKey1 a, TKey2 b, TKey3 c, TKey4 d)
        {
            return ContainsKey(Tuple.Create(a, b, c, d));
        }

        public bool ContainsKey1(TKey1 a)
        {
            return Keys.Any(x => Equals(x.Item1, a));
        }

        public bool ContainsKey2(TKey2 b)
        {
            return Keys.Any(x => Equals(x.Item2, b));
        }

        public bool ContainsKey3(TKey3 c)
        {
            return Keys.Any(x => Equals(x.Item3, c));
        }

        public bool ContainsKey4(TKey4 d)
        {
            return Keys.Any(x => Equals(x.Item2, d));
        }

        public ICollection<TKey1> Keys1 => Keys.Select(x => x.Item1).Distinct().ToList();
        public ICollection<TKey2> Keys2 => Keys.Select(x => x.Item2).Distinct().ToList();
        public ICollection<TKey3> Keys3 => Keys.Select(x => x.Item3).Distinct().ToList();
        public ICollection<TKey4> Keys4 => Keys.Select(x => x.Item4).Distinct().ToList();

        public void Add(TKey1 a, TKey2 b, TKey3 c, TKey4 d, TValue value)
        {
            Add(Tuple.Create(a, b, c, d), value);
        }

        public bool Remove(TKey1 a, TKey2 b, TKey3 c, TKey4 d)
        {
            return Remove(Tuple.Create(a, b, c, d));
        }

        public bool TryGetValue(TKey1 a, TKey2 b, TKey3 c, TKey4 d, out TValue value)
        {
            return TryGetValue(Tuple.Create(a, b, c, d), out value);
        }

        public TValue this[TKey1 a, TKey2 b, TKey3 c, TKey4 d]
        {
            get { return this[Tuple.Create(a, b, c, d)]; }
            set { this[Tuple.Create(a, b, c, d)] = value; }
        }

        public Maybe<TValue> GetMaybe(TKey1 a, TKey2 b, TKey3 c, TKey4 d)
        {
            TValue result;
            return TryGetValue(Tuple.Create(a, b, c, d), out result)
                ? Maybe.Some(result)
                : Maybe<TValue>.None;
        }
    }
}
