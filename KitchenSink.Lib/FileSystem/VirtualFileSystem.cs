namespace KitchenSink.FileSystem
{
    public class VirtualFileSystem : IFileSystem
    {
        public IFileOperations File { get; }
        public IDirectoryOperations Directory { get; }
        public IPathOperations Path { get; }
    }
}
