using System;

namespace KitchenSink
{
    // ReSharper disable UnusedTypeParameter
    public interface Sig<A> { }
    public interface Sig<A, B> { }
    public interface Sig<A, B, C> { }
    public interface Sig<A, B, C, D> { }
    public interface Sig<A, B, C, D, E> { }
    // ReSharper restore UnusedTypeParameter

    public static class Sig
    {
        public static Sig<A> Of<A>() => default(Sig<A>);
        public static Sig<A, B> Of<A, B>() => default(Sig<A, B>);
        public static Sig<A, B, C> Of<A, B, C>() => default(Sig<A, B, C>);
    }

    public class Test
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(Pick(Tuple.Create(true, DateTime.Now, 2.0m, 1, "a"), Sig.Of<bool, string>()));
        }

        public static A Pick<A, B>(Tuple<A, B> t, Sig<A> _) => t.Item1;
        public static B Pick<A, B>(Tuple<A, B> t, Sig<B> _) => t.Item2;
        public static Tuple<A, B> Pick<A, B>(Tuple<A, B> t, Sig<A, B> _) => t;

        public static A Pick<A, B, C>(Tuple<A, B, C> t, Sig<A> _) => t.Item1;
        public static B Pick<A, B, C>(Tuple<A, B, C> t, Sig<B> _) => t.Item2;
        public static C Pick<A, B, C>(Tuple<A, B, C> t, Sig<C> _) => t.Item3;
        public static Tuple<A, B> Pick<A, B, C>(Tuple<A, B, C> t, Sig<A, B> _) => Tuple.Create(t.Item1, t.Item2);
        public static Tuple<A, C> Pick<A, B, C>(Tuple<A, B, C> t, Sig<A, C> _) => Tuple.Create(t.Item1, t.Item3);
        public static Tuple<B, C> Pick<A, B, C>(Tuple<A, B, C> t, Sig<B, C> _) => Tuple.Create(t.Item2, t.Item3);
        public static Tuple<A, B, C> Pick<A, B, C>(Tuple<A, B, C> t, Sig<A, B, C> _) => t;

        public static A Pick<A, B, C, D>(Tuple<A, B, C, D> t, Sig<A> _) => t.Item1;
        public static B Pick<A, B, C, D>(Tuple<A, B, C, D> t, Sig<B> _) => t.Item2;
        public static C Pick<A, B, C, D>(Tuple<A, B, C, D> t, Sig<C> _) => t.Item3;
        public static D Pick<A, B, C, D>(Tuple<A, B, C, D> t, Sig<D> _) => t.Item4;
        public static Tuple<A, B> Pick<A, B, C, D>(Tuple<A, B, C, D> t, Sig<A, B> _) => Tuple.Create(t.Item1, t.Item2);
        public static Tuple<A, C> Pick<A, B, C, D>(Tuple<A, B, C, D> t, Sig<A, C> _) => Tuple.Create(t.Item1, t.Item3);
        public static Tuple<A, D> Pick<A, B, C, D>(Tuple<A, B, C, D> t, Sig<A, D> _) => Tuple.Create(t.Item1, t.Item4);
        public static Tuple<B, C> Pick<A, B, C, D>(Tuple<A, B, C, D> t, Sig<B, C> _) => Tuple.Create(t.Item2, t.Item3);
        public static Tuple<B, D> Pick<A, B, C, D>(Tuple<A, B, C, D> t, Sig<B, D> _) => Tuple.Create(t.Item2, t.Item4);
        public static Tuple<C, D> Pick<A, B, C, D>(Tuple<A, B, C, D> t, Sig<C, D> _) => Tuple.Create(t.Item3, t.Item4);
        public static Tuple<A, B, C> Pick<A, B, C, D>(Tuple<A, B, C, D> t, Sig<A, B, C> _) => Tuple.Create(t.Item1, t.Item2, t.Item3);
        public static Tuple<A, B, D> Pick<A, B, C, D>(Tuple<A, B, C, D> t, Sig<A, B, D> _) => Tuple.Create(t.Item1, t.Item2, t.Item4);
        public static Tuple<A, C, D> Pick<A, B, C, D>(Tuple<A, B, C, D> t, Sig<A, C, D> _) => Tuple.Create(t.Item1, t.Item3, t.Item4);
        public static Tuple<B, C, D> Pick<A, B, C, D>(Tuple<A, B, C, D> t, Sig<B, C, D> _) => Tuple.Create(t.Item2, t.Item3, t.Item4);
        public static Tuple<A, B, C, D> Pick<A, B, C, D>(Tuple<A, B, C, D> t, Sig<A, B, C, D> _) => t;

        public static A Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<A> _) => t.Item1;
        public static B Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<B> _) => t.Item2;
        public static C Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<C> _) => t.Item3;
        public static D Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<D> _) => t.Item4;
        public static E Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<E> _) => t.Item5;
        public static Tuple<A, B> Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<A, B> _) => Tuple.Create(t.Item1, t.Item2);
        public static Tuple<A, C> Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<A, C> _) => Tuple.Create(t.Item1, t.Item3);
        public static Tuple<A, D> Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<A, D> _) => Tuple.Create(t.Item1, t.Item4);
        public static Tuple<A, E> Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<A, E> _) => Tuple.Create(t.Item1, t.Item5);
        public static Tuple<B, C> Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<B, C> _) => Tuple.Create(t.Item2, t.Item3);
        public static Tuple<B, D> Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<B, D> _) => Tuple.Create(t.Item2, t.Item4);
        public static Tuple<B, E> Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<B, E> _) => Tuple.Create(t.Item2, t.Item5);
        public static Tuple<C, D> Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<C, D> _) => Tuple.Create(t.Item3, t.Item4);
        public static Tuple<C, E> Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<C, E> _) => Tuple.Create(t.Item3, t.Item5);
        public static Tuple<D, E> Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<D, E> _) => Tuple.Create(t.Item4, t.Item5);
        public static Tuple<A, B, C> Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<A, B, C> _) => Tuple.Create(t.Item1, t.Item2, t.Item3);
        public static Tuple<A, B, D> Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<A, B, D> _) => Tuple.Create(t.Item1, t.Item2, t.Item4);
        public static Tuple<A, B, E> Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<A, B, E> _) => Tuple.Create(t.Item1, t.Item2, t.Item5);
        public static Tuple<A, C, D> Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<A, C, D> _) => Tuple.Create(t.Item1, t.Item3, t.Item4);
        public static Tuple<A, C, E> Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<A, C, E> _) => Tuple.Create(t.Item1, t.Item3, t.Item5);
        public static Tuple<B, C, D> Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<B, C, D> _) => Tuple.Create(t.Item2, t.Item3, t.Item4);
        public static Tuple<B, C, E> Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<B, C, E> _) => Tuple.Create(t.Item2, t.Item3, t.Item5);
        public static Tuple<C, D, E> Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<C, D, E> _) => Tuple.Create(t.Item3, t.Item4, t.Item5);
        public static Tuple<A, B, C, D> Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<A, B, C, D> _) => Tuple.Create(t.Item1, t.Item2, t.Item3, t.Item4);
        public static Tuple<A, B, C, E> Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<A, B, C, E> _) => Tuple.Create(t.Item1, t.Item2, t.Item3, t.Item5);
        public static Tuple<A, B, D, E> Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<A, B, D, E> _) => Tuple.Create(t.Item1, t.Item2, t.Item4, t.Item5);
        public static Tuple<A, C, D, E> Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<A, C, D, E> _) => Tuple.Create(t.Item1, t.Item3, t.Item4, t.Item5);
        public static Tuple<B, C, D, E> Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<B, C, D, E> _) => Tuple.Create(t.Item2, t.Item3, t.Item4, t.Item5);
        public static Tuple<A, B, C, D, E> Pick<A, B, C, D, E>(Tuple<A, B, C, D, E> t, Sig<A, B, C, D, E> _) => t;
    }
}
