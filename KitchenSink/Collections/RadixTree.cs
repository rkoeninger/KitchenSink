using System;
using System.Collections.Generic;
using System.Linq;

namespace KitchenSink.Collections
{
    // TODO: implement IEnumerable<A>, IDictionary<string, A>, IReadOnlyDictionary<string, A>
    public class RadixTree<A>
    {
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
                else if (result.Parent != null)
                {
                    var edge = result.Parent.Edges
                        .FirstOrDefault(e => e.KeySegment.StartsWith(result.RemainingKey));

                    if (edge == null)
                    {
                        result.Parent.Edges
                            .Add(new Edge(result.RemainingKey, new Node(value)));
                    }
                    else
                    {
                        var childKey = edge.KeySegment.Substring(result.RemainingKey.Length);
                        result.Parent.Value = edge.Target.Value;
                        result.Parent.Edges.Add(new Edge(childKey, new Node(value)));
                    }
                }
                else
                {
                    root = new Node(value);
                }
            }
        }

        public void Add(string key, A value)
        {
            var result = Search(key);

            if (result.Target != null)
            {
                throw new ArgumentException($"Key {key} already present", key);
            }
            else if (result.Parent != null)
            {
                var edge = result.Parent.Edges
                    .FirstOrDefault(e => e.KeySegment.StartsWith(result.RemainingKey));

                if (edge == null)
                {
                    result.Parent.Edges
                        .Add(new Edge(result.RemainingKey, new Node(value)));
                }
                else
                {
                    var childKey = edge.KeySegment.Substring(result.RemainingKey.Length);
                    result.Parent.Value = edge.Target.Value;
                    result.Parent.Edges.Add(new Edge(childKey, new Node(value)));
                }
            }
            else
            {
                root = new Node(value);
            }
        }

        public bool Remove(string key)
        {
            // TODO: what to do here?
            return false;
        }

        private Node root;

        // Needs to return node, closest parent
        private SearchResults Search(string key)
        {
            if (root == null)
            {
                return new SearchResults(null, null, key);
            }

            Node parent = null;
            var current = root;
            var remainingKey = key;

            while (current != null && current.Edges.Count > 0 && remainingKey.Length > 0)
            {
                var edge = current.Edges.FirstOrDefault(e => remainingKey.StartsWith(e.KeySegment));

                if (edge == null)
                {
                    return new SearchResults(parent, null, remainingKey);
                }

                parent = current;
                current = edge.Target;
                remainingKey = remainingKey.Substring(edge.KeySegment.Length);
            }

            return new SearchResults(parent, remainingKey.Length == 0 ? current : null, remainingKey);
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
            public A Value;
            public readonly List<Edge> Edges;

            public Node(A value)
            {
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
