using System;
using System.Text;

namespace KitchenSink
{
    public interface IConsole
    {
        void WriteLine(string line);
    }

    public class RealConsole : IConsole
    {
        public void WriteLine(string line) => Console.WriteLine(line);
    }

    public class VirtualConsole : IConsole
    {
        private StringBuilder buffer = new StringBuilder();

        public void WriteLine(string line) => buffer.Append(line);
    }
}
