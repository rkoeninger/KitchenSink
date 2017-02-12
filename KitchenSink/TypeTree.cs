using System;
using System.Collections.Generic;
using System.Linq;

namespace KitchenSink
{
    public class TypeTree<A>
    {
        private class Node
        {
            public Node(Type key, A val)
            {
                Key = key;
                Value = val;
            }

            public readonly Type Key;
            public A Value;
            public List<Node> Children = new List<Node>();

            public bool IsRoot { get { return Key == null; } }

            public void InsertAbove(List<Node> children, Node middle)
            {
                Children = Children.Except(children).ToList();
                Children.Add(middle);
                middle.Children.AddRange(children);
            }
        }

        private readonly Node Root = new Node(null, default(A));

        private Node FindNode(Type key)
        {
            var current = Root;

            while (current.Key != key)
            {
                var next = current.Children.FirstOrDefault(x => x.Key.IsAssignableFrom(key));

                if (next == null)
                    return current;

                current = next;
            }

            return current;
        }

        public Maybe<A> Get(Type key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            var node = FindNode(key);
            return node.IsRoot ? Maybe<A>.None : node.Value;
        }

        public Maybe<A> Get<B>()
        {
            return Get(typeof(B));
        }

        public void Set(Type key, A val)
        {
            var closestNode = FindNode(key);

            if (closestNode.Key == key) // Exact match
            {
                closestNode.Value = val;
            }
            else
            {
                var newNode = new Node(key, val);
                var subtypeNodes = closestNode.Children.FindAll(x => x.Key.IsSubclassOf(key));

                if (subtypeNodes.Count == 0)
                {
                    if (key.IsInterface)
                    {
                        var firstInterfaceIndex = closestNode.Children.FindIndex(x => x.Key.IsInterface);

                        if (firstInterfaceIndex < 0)
                        {
                            closestNode.Children.Add(newNode);
                        }
                        else
                        {
                            closestNode.Children.Insert(firstInterfaceIndex, newNode);
                        }
                    }
                    else
                    {
                        closestNode.Children.Insert(0, newNode);
                    }
                }
                else
                {
                    closestNode.InsertAbove(subtypeNodes, newNode);
                }
            }
        }

        public void Set<B>(A val)
        {
            Set(typeof(B), val);
        }
    }
}
