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
        void Move(string source, string destination);
        EntryInfo GetInfo(string path);
        IEnumerable<EntryInfo> ReadDirectory(string path);
        Stream ReadFile(string path);
        Stream WriteFile(string path, bool append = false);
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

    public static class FileSystemOperations
    {
        public static bool Exists(this IFileSystem fs, string path) =>
            fs.GetInfo(path) != null;

        public static bool DirectoryExists(this IFileSystem fs, string path) =>
            fs.GetInfo(path)?.Type == EntryType.Directory;

        public static bool FileExists(this IFileSystem fs, string path) =>
            fs.GetInfo(path)?.Type == EntryType.File;

        public static void Clear(this IFileSystem fs, string path)
        {
            var entry = fs.GetInfo(path);

            if (entry?.IsFile ?? false)
            {
                fs.WriteFile(path).Close();
            }
            else if (entry?.IsDirectory ?? false)
            {
                foreach (var child in fs.ReadDirectory(path))
                {
                    fs.Delete(child.Path);
                }
            }
        }

        public static void Copy(this IFileSystem fs, string source, string destination)
        {
            var entry = fs.GetInfo(source);

            if (entry?.IsFile ?? false)
            {
                fs.ReadFile(source).Use(s => fs.WriteFile(destination).Use(s.CopyTo));
            }
            else if (entry?.IsDirectory ?? false)
            {
                fs.Create(EntryType.Directory, destination);

                foreach (var child in fs.ReadDirectory(source))
                {
                    fs.Copy(child.Path, Path.Combine(destination, child.Path.Substring(source.Length)));
                }
            }
            else
            {
                throw new PathNotFoundException(source);
            }
        }

        public static IEnumerable<EntryInfo> ReadDirectoryRecursive(this IFileSystem fs, string path)
        {
            foreach (var entry in fs.ReadDirectory(path))
            {
                yield return entry;

                if (entry.Type != EntryType.Directory) continue;

                foreach (var child in fs.ReadDirectoryRecursive(entry.Path))
                {
                    yield return child;
                }
            }
        }

        public static IEnumerable<byte> ReadBytes(this IFileSystem fs, string path) =>
            fs.ReadFile(path).AsEnumerable();

        public static IEnumerable<char> ReadChars(this IFileSystem fs, string path, Encoding encoding = null) =>
            fs.ReadFile(path).AsReader(encoding).AsEnumerableChars();

        public static IEnumerable<string> ReadLines(this IFileSystem fs, string path, Encoding encoding = null) =>
            fs.ReadFile(path).AsReader(encoding).AsEnumerableLines();

        public static string ReadText(this IFileSystem fs, string path, Encoding encoding = null) =>
            fs.ReadFile(path).Use(s => s.ReadTextToEnd(encoding));

        public static void WriteBytes(this IFileSystem fs, string path, IEnumerable<byte> bytes)
        {
            using var stream = bytes.ToStream();
            fs.WriteFile(path).Use(stream.CopyTo);
        }

        public static void WriteChars(this IFileSystem fs, string path, IEnumerable<char> chars, Encoding encoding = null) =>
            fs.WriteFile(path).AsWriter(encoding).Use(s => chars.ForEach(s.Write));

        public static void WriteLines(this IFileSystem fs, string path, IEnumerable<string> lines, Encoding encoding = null) =>
            fs.WriteFile(path).Use(s => lines.ForEach(s.AsWriter(encoding).WriteLine));

        public static void WriteText(this IFileSystem fs, string path, string text, Encoding encoding = null) =>
            fs.WriteFile(path).Use(s => s.AsWriter(encoding).Write(text));
    }
}
