using SystemFile = System.IO.File;
using SystemDirectory = System.IO.Directory;
using SystemPath = System.IO.Path;

namespace KitchenSink.FileSystem
{
    public class RealFileSystem : IFileSystem
    {
        public IFileOperations File { get; } = new RealFileOperations();
        public IDirectoryOperations Directory { get; } = new RealDirectoryOperations();
        public IPathOperations Path { get; } = new RealPathOperations();

        private class RealFileOperations : IFileOperations
        {
            public void Delete(string path) => SystemFile.Delete(path);
            public void Move(string sourceFileName, string destFileName) => SystemFile.Move(sourceFileName, destFileName);
        }

        private class RealDirectoryOperations : IDirectoryOperations
        {
            public void Delete(string path) => SystemDirectory.Delete(path);
        }

        private class RealPathOperations : IPathOperations
        {
            public char DirectorySeparatorChar => SystemPath.DirectorySeparatorChar;
            public char AltDirectorySeparatorChar => SystemPath.AltDirectorySeparatorChar;
            public char VolumeSeparatorChar => SystemPath.VolumeSeparatorChar;
            public char PathSeparator => SystemPath.PathSeparator;
            public char[] InvalidFileNameChars => SystemPath.GetInvalidFileNameChars();
            public char[] InvalidPathChars => SystemPath.GetInvalidPathChars();
            public string ChangeExtension(string path, string extension) => SystemPath.ChangeExtension(path, extension);
            public string Combine(params string[] paths) => SystemPath.Combine(paths);
            public string GetDirectoryName(string path) => SystemPath.GetDirectoryName(path);
            public string GetExtension(string path) => SystemPath.GetExtension(path);
            public string GetFileName(string path) => SystemPath.GetFileName(path);
            public string GetFileNameWithoutExtension(string path) => SystemPath.GetFileNameWithoutExtension(path);
            public string GetFullPath(string path) => SystemPath.GetFullPath(path);
            public string GetPathRoot(string path) => SystemPath.GetPathRoot(path);
            public string GetRandomFileName() => SystemPath.GetRandomFileName();
            public string GetTempFileName() => SystemPath.GetTempFileName();
            public string GetTempPath() => SystemPath.GetTempPath();
            public bool HasExtension(string path) => SystemPath.HasExtension(path);
            public bool IsPathRooted(string path) => SystemPath.IsPathRooted(path);
        }
    }
}
