using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KitchenSink.Collections;
using static KitchenSink.Operators;

namespace KitchenSink.FileSystem
{
    public class VirtualFileSystem : IFileSystem
    {
        public void Create(EntryType entry, string path) => throw new NotImplementedException();

        public void Delete(string path)
        {
            var node = Lookup(Parse(path));

            if (node?.Parent is DirectoryNode parent)
            {
                parent.Children.Remove(node);
            }
        }

        public void Move(string source, string destination) => throw new NotImplementedException();

        public EntryInfo GetInfo(string path)
        {
            var node = Lookup(Parse(path));
            return node == null ? null : new EntryInfo(node.Name, "", node.Type);
        }

        public IEnumerable<EntryInfo> ReadDirectory(string path)
        {
            return Lookup(Parse(path)) is DirectoryNode dir
                ? dir.Children.Select(e => new EntryInfo(e.Name, "", e.Type))
                : SeqOf<EntryInfo>();
        }

        public Stream ReadFile(string path) => throw new NotImplementedException();
        public Stream WriteFile(string path) => throw new NotImplementedException();
        public Stream AppendFile(string path) => throw new NotImplementedException();

        private readonly Node root = new DirectoryNode();

        private List<string> Parse(string path) =>
            path.Split('/')
                .Where(p => p != "." && !string.IsNullOrEmpty(p))
                .Aggregate(ConsList.Empty<string>(), (parts, p) =>
                    p == ".." && parts.Count > 0 ? parts.Tail : parts.Cons(p))
                .Reverse()
                .ToList();

        private Node Lookup(List<string> path) =>
            path.Aggregate(root, (current, name) => (current as DirectoryNode)?.Child(name));

        private class Node
        {
            public Node Parent { get; set; }
            public string Name { get; set; }
            public EntryType Type { get; set; }
            public DateTime Created { get; set; } = DateTime.Now;
            public DateTime Modified { get; set; } = DateTime.Now;
        }

        private class FileNode : Node
        {
            public FileNode() => Type = EntryType.File;
            public byte[] Content { get; } = new byte[0];
        }

        private class DirectoryNode : Node
        {
            public DirectoryNode() => Type = EntryType.Directory;
            public List<Node> Children { get; } = new List<Node>();
            public Node Child(string name) => Children.FirstOrDefault(x => x.Name == name);
        }
    }
}
