using System;
using System.Text;
using KitchenSink.Collections;

namespace KitchenSink
{
    public interface IConsole
    {
        void Write(string s);
        void WriteLine(string line);
        string ReadLine();
    }

    public class RealConsole : IConsole
    {
        public void Write(string s) => Console.Write(s);
        public void WriteLine(string line) => Console.WriteLine(line);
        public string ReadLine() => Console.ReadLine();
    }

    public class VirtualConsole : IConsole
    {
        public AsyncQueue<string> Input { get; } = new AsyncQueue<string>();
        public StringBuilder Output { get; } = new StringBuilder();

        public void Write(string s) => Output.Append(s);
        public void WriteLine(string line) => Output.AppendLine(line);
        public string ReadLine() => Input.DequeueAsync().Result;
    }
}
