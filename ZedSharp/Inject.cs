using System;
using System.IO;

namespace ZedSharp
{
    public static class Inject
    {
        public static readonly ConsoleDI StandardConsole = new StdConsole();

        private class StdConsole : ConsoleDI
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

        public static readonly FileSystemDI StandardFileSystem = new StdFileSystem();

        private class StdFileSystem : FileSystemDI
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
    }

    public interface ConsoleDI
    {
        String ReadLine();
        void WriteLine(Object s);
        void WriteLine(String format, params Object[] args);
    }

    public interface FileSystemDI
    {
        String ReadAllText(String path);
        void WriteAllText(String path, String contents);
    }
}
