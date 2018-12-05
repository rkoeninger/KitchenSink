namespace KitchenSink.FileSystem
{
    public interface IFileSystem
    {
        IFileOperations File { get; }
        IDirectoryOperations Directory { get; }
        IPathOperations Path { get; }
    }

    public interface IFileOperations
    {
        void Delete(string path);
        void Move(string sourceFileName, string destFileName);
    }

    public interface IDirectoryOperations
    {
        void Delete(string path);
    }

    public interface IPathOperations
    {
        char DirectorySeparatorChar { get; }
        char AltDirectorySeparatorChar { get; }
        char VolumeSeparatorChar { get; }
        char PathSeparator { get; }
        char[] InvalidFileNameChars { get; }
        char[] InvalidPathChars { get; }
        string ChangeExtension(string path, string extension);
        string Combine(params string[] paths);
        string GetDirectoryName(string path);
        string GetExtension(string path);
        string GetFileName(string path);
        string GetFileNameWithoutExtension(string path);
        string GetFullPath(string path);
        string GetPathRoot(string path);
        string GetRandomFileName();
        string GetTempFileName();
        string GetTempPath();
        bool HasExtension(string path);
        bool IsPathRooted(string path);
    }
}
