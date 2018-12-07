using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KitchenSink.FileSystem
{
    public class VirtualFileSystem : IFileSystem
    {
        public bool Exists(string path) => throw new NotImplementedException();
        public bool DirectoryExists(string path) => throw new NotImplementedException();
        public bool FileExists(string path) => throw new NotImplementedException();
        public void CreateDirectory(string path) => throw new NotImplementedException();
        public Stream CreateFile(string path) => throw new NotImplementedException();
        public void Delete(string path) => throw new NotImplementedException();
        public void Copy(string source, string destination) => throw new NotImplementedException();
        public void Move(string source, string destination) => throw new NotImplementedException();
        public IEnumerable<EntryInfo> ReadDirectory(string path) => throw new NotImplementedException();
        public Stream ReadFile(string path) => throw new NotImplementedException();
        public Stream WriteFile(string path) => throw new NotImplementedException();
        public Stream AppendFile(string path) => throw new NotImplementedException();

        private readonly List<Node> roots = new List<Node>();

        private List<string> Parse(string path)
        {
            // TODO: UNC paths

            return path.Split('/')
                .Where(p => p != "." && !string.IsNullOrEmpty(p))
                .Aggregate(new List<string>(), (parts, p) =>
                {
                    if (p == ".." && parts.Count > 0)
                    {
                        parts.RemoveAt(parts.Count - 1);
                    }
                    else
                    {
                        parts.Add(p);
                    }

                    return parts;
                });
        }

        private class Node
        {
            public string Name { get; set; }
            public DateTime Created { get; set; }
            public DateTime Modified { get; set; }
        }

        private class FileEntry : Node
        {
            public byte[] Content { get; } = new byte[0];
        }

        private class DirectoryEntry : Node
        {
            public IList<Node> Children { get; } = new List<Node>();
        }
    }
}
