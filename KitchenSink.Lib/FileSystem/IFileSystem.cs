using System.Collections.Generic;
using System.IO;

namespace KitchenSink.FileSystem
{
    public interface IFileSystem
    {
        bool Exists(string path);
        bool DirectoryExists(string path);
        bool FileExists(string path);
        void Copy(string source, string destination);
        Stream CreateFile(string path);
        void CreateDirectory(string path);
        void Delete(string path);
        void Move(string source, string destination);
        IEnumerable<EntryInfo> ReadDirectory(string path);
        Stream ReadFile(string path);
        Stream WriteFile(string path);
        Stream AppendFile(string path);
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
    }

    public enum EntryType
    {
        Directory,
        File
    }
}
