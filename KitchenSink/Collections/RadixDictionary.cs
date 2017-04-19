using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KitchenSink.Collections
{
    public class RadixDictionary<A> : IDictionary<string, A>, IReadOnlyDictionary<string, A>
    {
        private readonly Node root = new Node(null, default(A));

        public A this[string key]
        {
            get
            {
                var result = Search(key);

                if (result.Target == null)
                {
                    throw new KeyNotFoundException($"Key not found: {key}");
                }

                return result.Target.Value;
            }
            set
            {
                var result = Search(key);

                if (result.Target != null)
                {
                    result.Target.Value = value;
                }

                AddNew(key, value, result);
            }
        }

        public void Add(string key, A value)
        {
            var result = Search(key);

            if (result.Target != null)
            {
                throw new ArgumentException($"Key {key} already present", key);
            }

            AddNew(key, value, result);
        }

        public void Add(KeyValuePair<string, A> pair)
        {
            Add(pair.Key, pair.Value);
        }

        private void AddNew(string key, A value, SearchResults result)
        {
            var edge = result.Parent.Edges
                .FirstOrDefault(e => e.KeySegment.StartsWith(result.RemainingKey));

            if (edge == null)
            {
                result.Parent.Edges
                    .Add(new Edge(result.RemainingKey, new Node(key, value)));
            }
            else
            {
                var childKey = edge.KeySegment.Substring(result.RemainingKey.Length);
                result.Parent.Value = edge.Target.Value;
                result.Parent.Edges.Add(new Edge(childKey, new Node(key, value)));
            }
        }

        public bool Remove(string key)
        {
            var result = Search(key);

            if (result.Target == null)
            {
                return false;
            }

            var edgeIndex = result.Parent.Edges
                .FindIndex(e => key.EndsWith(e.KeySegment));

            if (edgeIndex < 0)
            {
                return false;
            }

            // TODO: remove parent if last edge removed
            result.Parent.Edges.RemoveAt(edgeIndex);
            return true;
        }

        public bool Remove(KeyValuePair<string, A> pair)
        {
            var result = Search(pair.Key);

            if (result.Target == null)
            {
                return false;
            }

            var edgeIndex = result.Parent.Edges
                .FindIndex(e => pair.Key.EndsWith(e.KeySegment));

            if (edgeIndex < 0)
            {
                return false;
            }

            var edge = result.Parent.Edges[edgeIndex];

            if (!Equals(pair.Value, edge.Target.Value))
            {
                return false;
            }

            // TODO: remove parent if last edge removed
            result.Parent.Edges.RemoveAt(edgeIndex);
            return true;
        }

        public void Clear()
        {
            root.Edges.Clear();
        }

        public ICollection<string> Keys => Enumerate().Select(x => x.Key).ToList();

        IEnumerable<string> IReadOnlyDictionary<string, A>.Keys => Enumerate().Select(x => x.Key);

        public ICollection<A> Values => Enumerate().Select(x => x.Value).ToList();

        IEnumerable<A> IReadOnlyDictionary<string, A>.Values => Enumerate().Select(x => x.Value);

        public int Count => Enumerate().Count();

        public bool IsReadOnly => false;

        public bool ContainsKey(string key) => Search(key).Target != null;

        public bool Contains(KeyValuePair<string, A> pair)
        {
            A value;
            return TryGetValue(pair.Key, out value) && Equals(value, pair.Value);
        }

        public bool TryGetValue(string key, out A value)
        {
            var result = Search(key);

            if (result.Target == null)
            {
                value = default(A);
                return false;
            }

            value = result.Target.Value;
            return true;
        }

        public IEnumerator<KeyValuePair<string, A>> GetEnumerator() => Enumerate().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void CopyTo(KeyValuePair<string, A>[] array, int arrayIndex)
        {
            foreach (var pair in Enumerate())
            {
                array[arrayIndex++] = pair;
            }
        }

        private SearchResults Search(string key)
        {
            if (root == null)
            {
                return new SearchResults(null, null, key);
            }

            Node parent = root;
            var current = root;
            var remainingKey = key;

            while (current != null && current.Edges.Count > 0 && remainingKey.Length > 0)
            {
                var edge = current.Edges
                    .FirstOrDefault(e => remainingKey.StartsWith(e.KeySegment));

                if (edge == null)
                {
                    return new SearchResults(parent, null, remainingKey);
                }

                parent = current;
                current = edge.Target;
                remainingKey = remainingKey.Substring(edge.KeySegment.Length);
            }

            return new SearchResults(
                parent,
                remainingKey.Length == 0 ? current : null,
                remainingKey);
        }

        private IEnumerable<KeyValuePair<string, A>> Enumerate()
        {
            foreach (var edge in root.Edges)
            {
                foreach (var pair in Enumerate(edge))
                {
                    yield return pair;
                }
            }
        }

        private IEnumerable<KeyValuePair<string, A>> Enumerate(Edge rootEdge)
        {
            yield return new KeyValuePair<string, A>(rootEdge.Target.Key, rootEdge.Target.Value);

            foreach (var edge in rootEdge.Target.Edges)
            {
                foreach (var pair in Enumerate(edge))
                {
                    yield return pair;
                }
            }
        }

        private struct SearchResults
        {
            public readonly Node Parent;
            public readonly Node Target;
            public readonly string RemainingKey;

            public SearchResults(Node parent, Node target, string remainingKey)
            {
                Parent = parent;
                Target = target;
                RemainingKey = remainingKey;
            }
        }

        private class Node
        {
            public string Key;
            public A Value;
            public readonly List<Edge> Edges;

            public Node(string key, A value)
            {
                Key = key;
                Value = value;
                Edges = new List<Edge>();
            }
        }

        private class Edge
        {
            public readonly string KeySegment;
            public readonly Node Target;

            public Edge(string keySegment, Node target)
            {
                KeySegment = keySegment;
                Target = target;
            }
        }
    }
}
