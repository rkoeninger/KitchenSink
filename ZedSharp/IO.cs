using System;
using System.IO;

namespace ZedSharp
{
    public static class IO
    {
        public static IO<A> Of<A>(A val)
        {
            return new IO<A>(() => val);
        }

        public static IO<A> Of<A>(Func<A> cont)
        {
            return new IO<A>(cont);
        }

        public static IO<Unit> Of_(Action cont)
        {
            return new IO<Unit>(() => { cont(); return Unit.It; });
        }

        public static IO<A> Flatten<A>(this IO<IO<A>> io)
        {
            return IO.Of(io.Eval().Cont);
        }
    }

    public struct IO<A>
    {
        internal IO(Func<A> cont) : this()
        {
            Cont = cont;
        }

        internal Func<A> Cont { get; set; }

        public A Eval()
        {
            return Cont();
        }

        public IO<B> Select<B>(Func<A, B> f)
        {
            var me = this;
            return IO.Of(() => f(me.Eval()));
        }

        public IO<B> SelectMany<B>(Func<A, IO<B>> f)
        {
            var me = this;
            return IO.Of(() => f(me.Eval()).Eval());
        }

        public IO<B> Then<B>(IO<B> io)
        {
            var me = this;
            return IO.Of(() => { me.Eval(); return io.Eval(); });
        }
    }

    public static class ConsoleIO
    {
        public static readonly IO<String> ReadLine = IO.Of(Console.ReadLine);
        public static readonly IO<Int32> ReadChar = IO.Of(Console.Read);
        public static readonly IO<ConsoleKeyInfo> ReadKey = IO.Of(Console.ReadKey);
        
        public static IO<Unit> Write(Object s)
        {
            return IO.Of_(() => Console.Write(s));
        }

        public static IO<Unit> Write(String format, params Object[] args)
        {
            return IO.Of_(() => Console.Write(format, args));
        }
        
        public static IO<Unit> WriteLine(Object s)
        {
            return IO.Of_(() => Console.WriteLine(s));
        }

        public static IO<Unit> WriteLine(String format, params Object[] args)
        {
            return IO.Of_(() => Console.WriteLine(format, args));
        }
    }

    public static class FileIO
    {
        public static IO<String> ReadAllText(String path)
        {
            return IO.Of(() => File.ReadAllText(path));
        }

        public static IO<String[]> ReadAllLines(String path)
        {
            return IO.Of(() => File.ReadAllLines(path));
        }

        public static IO<byte[]> ReadAllBytes(String path)
        {
            return IO.Of(() => File.ReadAllBytes(path));
        }

        public static IO<Unit> WriteAllText(String path, String contents)
        {
            return IO.Of_(() => File.WriteAllText(path, contents));
        }

        public static IO<Unit> WriteAllLines(String path, String[] contents)
        {
            return IO.Of_(() => File.WriteAllLines(path, contents));
        }

        public static IO<Unit> WriteAllBytes(String path, byte[] bytes)
        {
            return IO.Of_(() => File.WriteAllBytes(path, bytes));
        }
    }
}
