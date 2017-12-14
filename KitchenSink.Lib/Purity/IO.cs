using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KitchenSink.Purity
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
            return Of(() => io.Eval().Eval());
        }

        public static IO<A> Sum<A>(this IEnumerable<IO<A>> seq)
        {
            return seq.Aggregate(Then);
        }

        public static IO<IEnumerable<A>> Sequence<A>(this IEnumerable<IO<A>> seq)
        {
            return Of(() => seq.Select(Eval));
        }

        public static IO<Unit> Sequence(this IEnumerable<IO<Unit>> seq)
        {
            return Of(() =>
            {
                foreach (var io in seq)
                    io.Eval();

                return Unit.It;
            });
        }

        public static Func<A, IO<C>> Compose<A, B, C>(this Func<A, IO<B>> f, Func<B, IO<C>> g)
        {
            return a => f(a).SelectMany(g);
        }

        public static IO<Func<A, B>> Promote<A, B>(Func<A, IO<B>> f)
        {
            return Of<Func<A, B>>(() => a => f(a).Eval());
        }

        public static Func<A, IO<B>> Demote<A, B>(IO<Func<A, B>> f)
        {
            return x => Of(() => f.Eval().Invoke(x));
        }

        public static readonly IO<DateTime> Now = Of(() => DateTime.Now);
        public static readonly IO<DateTime> UtcNow = Of(() => DateTime.UtcNow);

        private static A Eval<A>(IO<A> io)
        {
            return io.Eval();
        }

        private static IO<B> Then<A, B>(IO<A> a, IO<B> b)
        {
            return a.Then(b);
        }

        private static IO<A> Also<A, B>(IO<A> a, IO<B> b)
        {
            return a.Also(b);
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
            return IO.Of(() =>
            {
                me.Eval();
                return io.Eval();
            });
        }

        public IO<A> Also<B>(IO<B> io)
        {
            return io.Then(this);
        }

        public IO<C> Join<B, C>(IO<B> other, Func<A, B, C> f)
        {
            var me = this;
            return IO.Of(() => f(me.Eval(), other.Eval()));
        }

        public IO<Unit> Forever()
        {
            return Forever<Unit>();
        }

        public IO<B> Forever<B>()
        {
            var me = this;

            // ReSharper disable once FunctionNeverReturns
            return IO.Of<B>(() => { while (true) me.Eval(); });
        }

        public IO<Unit> Ignore()
        {
            var me = this;
            return IO.Of_(() => me.Eval());
        }
    }

    public static class ConsoleIO
    {
        public static readonly IO<string> ReadLine = IO.Of(Console.ReadLine);
        public static readonly IO<int> ReadChar = IO.Of(Console.Read);
        public static readonly IO<ConsoleKeyInfo> ReadKey = IO.Of(Console.ReadKey);
        
        public static IO<Unit> Write(object s)
        {
            return IO.Of_(() => Console.Write(s));
        }

        public static IO<Unit> Write(string format, params object[] args)
        {
            return IO.Of_(() => Console.Write(format, args));
        }
        
        public static IO<Unit> WriteLine(object s)
        {
            return IO.Of_(() => Console.WriteLine(s));
        }

        public static IO<Unit> WriteLine(string format, params object[] args)
        {
            return IO.Of_(() => Console.WriteLine(format, args));
        }
    }

    public static class FileIO
    {
        public static IO<string> ReadAllText(string path)
        {
            return IO.Of(() => File.ReadAllText(path));
        }

        public static IO<string[]> ReadAllLines(string path)
        {
            return IO.Of(() => File.ReadAllLines(path));
        }

        public static IO<byte[]> ReadAllBytes(string path)
        {
            return IO.Of(() => File.ReadAllBytes(path));
        }

        public static IO<Unit> WriteAllText(string path, string contents)
        {
            return IO.Of_(() => File.WriteAllText(path, contents));
        }

        public static IO<Unit> WriteAllLines(string path, string[] contents)
        {
            return IO.Of_(() => File.WriteAllLines(path, contents));
        }

        public static IO<Unit> WriteAllBytes(string path, byte[] bytes)
        {
            return IO.Of_(() => File.WriteAllBytes(path, bytes));
        }
    }
}
