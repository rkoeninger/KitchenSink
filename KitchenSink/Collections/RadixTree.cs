using System.Collections.Generic;
using System.Linq;
using static KitchenSink.Operators;

namespace KitchenSink.Collections
{
    // TODO: implement IEnumerable<A>, IDictionary<string, A>, IReadOnlyDictionary<string, A>
    public class RadixTree<A>
    {
        public A this[string key]
        {
            get
            {
                return Search(key)
                    .OrElseThrow(new KeyNotFoundException($"Key not found: {key}"))
                    .Value;
            }
            set
            {
                Search(key)
                    .Branch(
                        node => node.Value = value,
                        () => { }); // TODO: what to do here?
            }
        }

        public void Add(string key, A value)
        {
            // TODO: what to do here?
        }

        public void Remove(string key)
        {
            // TODO: what to do here?
        }

        private Node root;
        private readonly Maybe<Node> NoneNode = None<Node>();

        // Needs to return node, closest parent
        private Maybe<Node> Search(string key)
        {
            if (root == null)
            {
                return NoneNode;
            }

            var current = root;
            var remainingKey = key;

            while (current != null && current.Edges.Count > 0 && remainingKey.Length > 0)
            {
                var edge = current.Edges.FirstOrDefault(e => remainingKey.StartsWith(e.KeySegment));

                if (edge == null)
                {
                    return NoneNode;
                }

                current = edge.Target;
                remainingKey = remainingKey.Substring(edge.KeySegment.Length);
            }

            return remainingKey.Length == 0 ? Some(current) : NoneNode;
        }

        private class SearchResults
        {
            public readonly Maybe<Node> Parent;
            public readonly Maybe<Node> Result;

            public SearchResults(Maybe<Node> parent, Maybe<Node> result)
            {
                Parent = parent;
                Result = result;
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
