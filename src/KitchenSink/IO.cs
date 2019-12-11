using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KitchenSink
{
    public static class Io
    {
        public static Io<A> Of<A>(A val) => new Io<A>(() => val);

        public static Io<A> Of<A>(Func<A> cont) => new Io<A>(cont);

        public static Io<Unit> Of_(Action cont) =>
            new Io<Unit>(() => { cont(); return Unit.It; });

        public static Io<A> Flatten<A>(this Io<Io<A>> io) => Of(() => io.Eval().Eval());

        public static Io<A> Sum<A>(this IEnumerable<Io<A>> seq) => seq.Aggregate(Then);

        public static Io<IEnumerable<A>> Sequence<A>(this IEnumerable<Io<A>> seq) =>
            Of(() => seq.Select(Eval));

        public static Io<Unit> Sequence(this IEnumerable<Io<Unit>> seq) =>
            Of(() =>
            {
                foreach (var io in seq)
                {
                    io.Eval();
                }

                return Unit.It;
            });

        public static Func<A, Io<C>> Compose<A, B, C>(this Func<A, Io<B>> f, Func<B, Io<C>> g) =>
            a => f(a).SelectMany(g);

        public static Io<Func<A, B>> Promote<A, B>(Func<A, Io<B>> f) =>
            Of<Func<A, B>>(() => a => f(a).Eval());

        public static Func<A, Io<B>> Demote<A, B>(Io<Func<A, B>> f) =>
            x => Of(() => f.Eval().Invoke(x));

        public static readonly Io<DateTime> Now = Of(() => DateTime.Now);
        public static readonly Io<DateTime> UtcNow = Of(() => DateTime.UtcNow);

        private static A Eval<A>(Io<A> io) => io.Eval();

        public static Io<B> Then<A, B>(Io<A> a, Io<B> b) => a.Then(b);

        public static Io<A> Also<A, B>(Io<A> a, Io<B> b) => a.Also(b);
    }

    public struct Io<A>
    {
        internal Io(Func<A> cont) : this() => Cont = cont;

        internal Func<A> Cont { get; }

        public A Eval() => Cont();

        public Io<B> Select<B>(Func<A, B> f)
        {
            var me = this;
            return Io.Of(() => f(me.Eval()));
        }

        public Io<B> SelectMany<B>(Func<A, Io<B>> f)
        {
            var me = this;
            return Io.Of(() => f(me.Eval()).Eval());
        }

        public Io<B> Then<B>(Io<B> io)
        {
            var me = this;
            return Io.Of(() =>
            {
                me.Eval();
                return io.Eval();
            });
        }

        public Io<A> Also<B>(Io<B> io) => io.Then(this);

        public Io<C> Join<B, C>(Io<B> other, Func<A, B, C> f)
        {
            var me = this;
            return Io.Of(() => f(me.Eval(), other.Eval()));
        }

        public Io<Unit> Forever() => Forever<Unit>();

        public Io<B> Forever<B>()
        {
            var me = this;

            // ReSharper disable once FunctionNeverReturns
            return Io.Of<B>(() => { while (true) me.Eval(); });
        }

        public Io<Unit> Ignore()
        {
            var me = this;
            return Io.Of_(() => me.Eval());
        }
    }

    public static class ConsoleIo
    {
        public static readonly Io<string> ReadLine = Io.Of(Console.ReadLine);
        public static readonly Io<int> ReadChar = Io.Of(Console.Read);
        public static readonly Io<ConsoleKeyInfo> ReadKey = Io.Of(Console.ReadKey);

        public static Io<Unit> Write(object s) => Io.Of_(() => Console.Write(s));

        public static Io<Unit> Write(string format, params object[] args) =>
            Io.Of_(() => Console.Write(format, args));

        public static Io<Unit> WriteLine(object s) =>
            Io.Of_(() => Console.WriteLine(s));

        public static Io<Unit> WriteLine(string format, params object[] args) =>
            Io.Of_(() => Console.WriteLine(format, args));
    }

    public static class FileIo
    {
        public static Io<string> ReadAllText(string path) =>
            Io.Of(() => File.ReadAllText(path));

        public static Io<string[]> ReadAllLines(string path) =>
            Io.Of(() => File.ReadAllLines(path));

        public static Io<byte[]> ReadAllBytes(string path) =>
            Io.Of(() => File.ReadAllBytes(path));

        public static Io<Unit> WriteAllText(string path, string contents) =>
            Io.Of_(() => File.WriteAllText(path, contents));

        public static Io<Unit> WriteAllLines(string path, string[] contents) =>
            Io.Of_(() => File.WriteAllLines(path, contents));

        public static Io<Unit> WriteAllBytes(string path, byte[] bytes) =>
            Io.Of_(() => File.WriteAllBytes(path, bytes));
    }
}
