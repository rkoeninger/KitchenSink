using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KitchenSink.Collections;
using KitchenSink.Extensions;
using static KitchenSink.Operators;

namespace KitchenSink.FileSystem
{
    public class VirtualFileSystem : IFileSystem
    {
        public void Create(EntryType entry, string path)
        {
            var parsed = Parse(path);
            var parent = (DirectoryNode) Lookup(parsed.Take(parsed.Count - 1).ToList());
            var node = entry == EntryType.Directory ? (Node) new DirectoryNode() : new FileNode();
            node.Name = parsed.Last();
            node.Parent = parent;
            parent.Children.Add(node);
        }

        public void Delete(string path)
        {
            var node = Lookup(Parse(path));

            if (node?.Parent is DirectoryNode parent)
            {
                parent.Children.Remove(node);
            }
        }

        public void Move(string source, string destination)
        {
            var node = Lookup(Parse(source));
            var sourceParent = (DirectoryNode) node.Parent;
            var parsed = Parse(destination);
            var destinationParent = (DirectoryNode) Lookup(parsed.Take(parsed.Count - 1).ToList());
            sourceParent.Children.Remove(node);
            destinationParent.Children.Add(node);
            node.Parent = destinationParent;
            node.Name = parsed.Last();
        }

        public EntryInfo GetInfo(string path)
        {
            var parsed = Parse(path);
            var node = Lookup(parsed);
            return node == null ? null : new EntryInfo(node.Name, Print(parsed), node.Type);
        }

        public IEnumerable<EntryInfo> ReadDirectory(string path)
        {
            var parsed = Parse(path);
            return Lookup(parsed) is DirectoryNode dir
                ? dir.Children.Select(e => new EntryInfo(e.Name, Print(parsed), e.Type))
                : SeqOf<EntryInfo>();
        }

        public Stream ReadFile(string path) => ((FileNode) Lookup(Parse(path))).Data.ToStream();

        public Stream WriteFile(string path, bool append = false) => new WriteStream((FileNode) Lookup(Parse(path)), append);

        private readonly Node root = new DirectoryNode();

        private List<string> Parse(string path) =>
            path.Split('/')
                .Where(p => p != "." && !string.IsNullOrEmpty(p))
                .Aggregate(ConsList.Empty<string>(), (parts, p) =>
                    p == ".." && parts.Count > 0 ? parts.Tail : parts.Cons(p))
                .Reverse()
                .ToList();

        private string Print(IEnumerable<string> pathParts) =>
            pathParts.Select(p => "/" + p).MkStr();

        private Node Lookup(List<string> path) =>
            path.Aggregate(root, (current, name) => (current as DirectoryNode)?.Child(name));

        private abstract class Node
        {
            public abstract EntryType Type { get; }
            public Node Parent { get; set; }
            public string Name { get; set; }
            public DateTime Created { get; set; } = DateTime.Now;
            public DateTime Modified { get; set; } = DateTime.Now;
        }

        private class FileNode : Node
        {
            public override EntryType Type => EntryType.File;
            public byte[] Data { get; set; } = Array.Empty<byte>();
        }

        private class DirectoryNode : Node
        {
            public override EntryType Type => EntryType.Directory;
            public List<Node> Children { get; } = new List<Node>();
            public Node Child(string name) => Children.FirstOrDefault(x => x.Name == name);
        }

        private class WriteStream : MemoryStream
        {
            private readonly FileNode node;
            private readonly bool append;

            public WriteStream(FileNode node, bool append)
            {
                this.node = node;
                this.append = append;
            }

            public override void Flush()
            {
                node.Data = append ? node.Data.Concat(ToArray()) : ToArray();
            }
        }
    }
}
