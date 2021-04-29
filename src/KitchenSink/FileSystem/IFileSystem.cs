using System.Collections.Generic;
using System.IO;
using System.Text;
using KitchenSink.Extensions;

namespace KitchenSink.FileSystem
{
    public interface IFileSystem
    {
        void Create(EntryType type, string path);

        void Delete(string path);

        void DeleteRecursive(string path)
        {
            var entry = GetInfo(path);

            if (entry?.IsFile ?? false)
            {
                WriteFile(path).Close();
            }
            else if (entry?.IsDirectory ?? false)
            {
                foreach (var child in ReadDirectory(path))
                {
                    Delete(child.Path);
                }
            }
        }

        void Move(string source, string destination);

        void Copy(string source, string destination)
        {
            var entry = GetInfo(source);

            if (entry?.IsFile ?? false)
            {
                WriteBytes(destination, ReadBytes(source));
            }
            else if (entry?.IsDirectory ?? false)
            {
                Create(EntryType.Directory, destination);

                foreach (var child in ReadDirectory(source))
                {
                    Copy(child.Path, Path.Combine(destination, child.Path.Substring(source.Length)));
                }
            }
            else
            {
                throw new PathNotFoundException(source);
            }
        }

        EntryInfo GetInfo(string path);
        bool Exists(string path) => GetInfo(path) != null;
        bool DirectoryExists(string path) => GetInfo(path)?.Type == EntryType.Directory;
        bool FileExists(string path) => GetInfo(path)?.Type == EntryType.File;

        IEnumerable<EntryInfo> ReadDirectory(string path);

        IEnumerable<EntryInfo> ReadDirectoryRecursive(string path)
        {
            foreach (var entry in ReadDirectory(path))
            {
                yield return entry;

                if (entry.Type != EntryType.Directory) continue;

                foreach (var child in ReadDirectoryRecursive(entry.Path))
                {
                    yield return child;
                }
            }
        }

        Stream ReadFile(string path);

        IEnumerable<byte> ReadBytes(string path) => ReadFile(path).AsEnumerable();
        IEnumerable<char> ReadChars(string path, Encoding encoding = null) =>
            ReadFile(path).AsReader(encoding).AsEnumerableChars();
        IEnumerable<string> ReadLines(string path, Encoding encoding = null) =>
            ReadFile(path).AsReader(encoding).AsEnumerableLines();
        string ReadText(string path, Encoding encoding = null) =>
            ReadFile(path).Use(s => s.ReadTextToEnd(encoding));

        Stream WriteFile(string path, bool append = false);

        void WriteBytes(string path, IEnumerable<byte> bytes) =>
            bytes.ToStream().Use(s => WriteFile(path).Use(s.CopyTo));
        void WriteChars(string path, IEnumerable<char> chars, Encoding encoding = null) =>
            WriteFile(path).AsWriter(encoding).Use(s => chars.ForEach(s.Write));
        void WriteLines(string path, IEnumerable<string> lines, Encoding encoding = null) =>
            WriteFile(path).Use(s => lines.ForEach(s.AsWriter(encoding).WriteLine));
        void WriteText(string path, string text, Encoding encoding = null) =>
            WriteFile(path).Use(s => s.AsWriter(encoding).Write(text));
    }

    public class EntryInfo
    {
        public EntryInfo(string name, string path, EntryType type)
        {
            Name = name;
            Path = path;
            Type = type;
        }

        public string Name { get; }
        public string Path { get; }
        public EntryType Type { get; }
        public bool IsDirectory => Type == EntryType.Directory;
        public bool IsFile => Type == EntryType.File;
    }

    public enum EntryType
    {
        Directory,
        File
    }
}
