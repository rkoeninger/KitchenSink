using System.IO;

namespace KitchenSink.FileSystem
{
    public class PathNotFoundException : IOException
    {
        public PathNotFoundException(string path) : base($"No file or directory named \"{path}\"") {}
    }
}
