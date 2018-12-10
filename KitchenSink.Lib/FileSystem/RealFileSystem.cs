using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KitchenSink.FileSystem
{
    public class RealFileSystem : IFileSystem
    {
        public void Create(EntryType type, string path)
        {
            if (type == EntryType.Directory)
            {
                Directory.CreateDirectory(path);
            }
            else if (type == EntryType.File)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.Create(path).Close();
            }
            else
            {
                throw new ArgumentException($"Invalid EntryType: \"{type}\"");
            }
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

        public EntryInfo GetInfo(string path)
        {
            var fileName = Path.GetFileName(path);
            var fullPath = Path.GetFullPath(path);
            return
                File.Exists(path) ? new EntryInfo(fileName, fullPath, EntryType.File) :
                Directory.Exists(path) ? new EntryInfo(fileName, fullPath, EntryType.Directory) :
                null;
        }

        public IEnumerable<EntryInfo> ReadDirectory(string path) =>
            Directory.GetFileSystemEntries(path, "*", SearchOption.TopDirectoryOnly).Select(GetInfo);

        public Stream ReadFile(string path) => File.Exists(path) ? (Stream)File.OpenRead(path) : new MemoryStream();

        public Stream WriteFile(string path, bool append = false)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            var mode = append && File.Exists(path) ? FileMode.Append : FileMode.Create;
            return File.Open(path, mode, FileAccess.Write);
        }
    }
}
