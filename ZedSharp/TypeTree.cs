using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ZedSharp
{
    public class TypeTree<A>
    {
        [DebuggerDisplay("{Key} - {Children.Count} child nodes")]
        private class Node
        {
            public Node(Type key, A val, Node parent)
            {
                Key = key;
                Value = val;
                Parent = parent;
            }

            public Type Key;
            public A Value;
            public Node Parent;
            public List<Node> Children = new List<Node>();

            public bool IsRoot { get { return Key == null; } }

            public void InsertAbove(List<Node> children, Node middle)
            {
                Children = Children.Except(children).ToList();
                middle.Children.AddRange(children);
                children.ForEach(x => x.Parent = middle);

                Children.Add(middle);
                middle.Parent = this;
            }
        }

        private Node Root = new Node(null, default(A), null);

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

        public A Get(Type key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            var node = FindNode(key);

            if (node.IsRoot)
                throw new KeyNotFoundException("No value found for type " + key.Name);

            return node.Value;
        }

        public A Get<B>()
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
                var newNode = new Node(key, val, closestNode);
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
