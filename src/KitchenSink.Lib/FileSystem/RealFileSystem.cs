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

        public void Delete(string path) =>
            Branch(path,
                () => File.Delete(path),
                () => Directory.Delete(path, true));

        public void Move(string source, string destination)
        {
            if (!Branch(source,
                () => File.Move(source, destination),
                () => Directory.Move(source, destination)))
            {
                throw new PathNotFoundException(source);
            }
        }

        public EntryInfo GetInfo(string path)
        {
            EntryInfo Entry(EntryType type) => new EntryInfo(Path.GetFileName(path), Path.GetFullPath(path), type);
            return Branch(path,
                () => Entry(EntryType.File),
                () => Entry(EntryType.Directory));
        }

        private A Branch<A>(string path, Func<A> fileAction, Func<A> directoryAction) =>
            File.Exists(path) ? fileAction() : Directory.Exists(path) ? directoryAction() : default;

        private bool Branch(string path, Action fileAction, Action directoryAction)
        {
            if (File.Exists(path))
            {
                fileAction();
                return true;
            }
            else if (Directory.Exists(path))
            {
                directoryAction();
                return true;
            }
            else
            {
                return false;
            }
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
