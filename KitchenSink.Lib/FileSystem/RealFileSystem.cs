using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KitchenSink.FileSystem
{
    public class RealFileSystem : IFileSystem
    {
        public bool Exists(string path) => File.Exists(path) || Directory.Exists(path);
        public bool DirectoryExists(string path) => Directory.Exists(path);
        public bool FileExists(string path) => File.Exists(path);
        public void CreateDirectory(string path) => Directory.CreateDirectory(path);

        public Stream CreateFile(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            return File.Create(path);
        }

        public void Delete(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        public void Copy(string source, string destination)
        {
            if (File.Exists(source))
            {
                File.Copy(source, destination, true);
            }
            else if (Directory.Exists(source))
            {
                CreateDirectory(destination);

                foreach (var entry in Directory.GetFileSystemEntries(source, "*", SearchOption.AllDirectories))
                {
                    Copy(entry, Path.Combine(destination, entry.Substring(source.Length)));
                }
            }
            else
            {
                throw new PathNotFoundException(source);
            }
        }

        public void Move(string source, string destination)
        {
            if (File.Exists(source))
            {
                File.Move(source, destination);
            }
            else if (Directory.Exists(source))
            {
                Directory.Move(source, destination);
            }
            else
            {
                throw new PathNotFoundException(source);
            }
        }

        public IEnumerable<EntryInfo> ReadDirectory(string path) =>
            Directory.GetFiles(path, "*", SearchOption.AllDirectories)
                .Select(p => new EntryInfo(Path.GetFileName(p), p, EntryType.File))
                .Concat(Directory.GetDirectories(path, "*", SearchOption.AllDirectories)
                    .Select(p => new EntryInfo(Path.GetFileName(p), p, EntryType.Directory)));

        public Stream ReadFile(string path) => File.OpenRead(path);
        public Stream WriteFile(string path) => File.OpenWrite(path);
        public Stream AppendFile(string path) => File.Open(path, File.Exists(path) ? FileMode.Append : FileMode.Create);
    }
}
