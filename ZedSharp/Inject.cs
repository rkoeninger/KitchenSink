using System;
using System.IO;

namespace ZedSharp
{
    public static class Inject
    {
        public static readonly IConsole StandardConsole = new StdConsole();

        private class StdConsole : IConsole
        {
            public String ReadLine()
            {
                return Console.ReadLine();
            }

            public void WriteLine(Object s)
            {
                Console.WriteLine(s);
            }

            public void WriteLine(String format, params Object[] args)
            {
                Console.WriteLine(format, args);
            }
        }

        public static readonly IFileSystem StandardFileSystem = new StdFileSystem();

        private class StdFileSystem : IFileSystem
        {
            public String ReadAllText(String path)
            {
                return File.ReadAllText(path);
            }

            public void WriteAllText(String path, String contents)
            {
                File.WriteAllText(path, contents);
            }
        }

        public static readonly Needs StandardDeps = Needs.Of(
            StandardConsole,
            StandardFileSystem);
    }

    public interface IConsole
    {
        String ReadLine();
        void WriteLine(Object s);
        void WriteLine(String format, params Object[] args);
    }

    public interface IFileSystem
    {
        String ReadAllText(String path);
        void WriteAllText(String path, String contents);
    }
}
