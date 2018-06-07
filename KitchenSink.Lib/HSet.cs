using System;

namespace KitchenSink
{
    // ReSharper disable UnusedTypeParameter
    public interface ISig<A> { }
    public interface ISig<A, B> { }
    public interface ISig<A, B, C> { }
    public interface ISig<A, B, C, D> { }
    public interface ISig<A, B, C, D, E> { }
    // ReSharper restore UnusedTypeParameter

    public static class Sig
    {
        public static ISig<A> Of<A>() => default;
        public static ISig<A, B> Of<A, B>() => default;
        public static ISig<A, B, C> Of<A, B, C>() => default;
    }

    public static class HSet
    {
        public static A Pick<A, B>((A, B) t, ISig<A> _) => t.Item1;
        public static B Pick<A, B>((A, B) t, ISig<B> _) => t.Item2;
        public static (A, B) Pick<A, B>((A, B) t, ISig<A, B> _) => t;

        public static A Pick<A, B, C>((A, B, C) t, ISig<A> _) => t.Item1;
        public static B Pick<A, B, C>((A, B, C) t, ISig<B> _) => t.Item2;
        public static C Pick<A, B, C>((A, B, C) t, ISig<C> _) => t.Item3;
        public static (A, B) Pick<A, B, C>((A, B, C) t, ISig<A, B> _) => (t.Item1, t.Item2);
        public static (A, C) Pick<A, B, C>((A, B, C) t, ISig<A, C> _) => (t.Item1, t.Item3);
        public static (B, C) Pick<A, B, C>((A, B, C) t, ISig<B, C> _) => (t.Item2, t.Item3);
        public static (A, B, C) Pick<A, B, C>((A, B, C) t, ISig<A, B, C> _) => t;

        public static A Pick<A, B, C, D>(Tuple<A, B, C, D> t, ISig<A> _) => t.Item1;
        public static B Pick<A, B, C, D>(Tuple<A, B, C, D> t, ISig<B> _) => t.Item2;
        public static C Pick<A, B, C, D>(Tuple<A, B, C, D> t, ISig<C> _) => t.Item3;
        public static D Pick<A, B, C, D>(Tuple<A, B, C, D> t, ISig<D> _) => t.Item4;
        public static (A, B) Pick<A, B, C, D>((A, B, C, D) t, ISig<A, B> _) => (t.Item1, t.Item2);
        public static (A, C) Pick<A, B, C, D>((A, B, C, D) t, ISig<A, C> _) => (t.Item1, t.Item3);
        public static (A, D) Pick<A, B, C, D>((A, B, C, D) t, ISig<A, D> _) => (t.Item1, t.Item4);
        public static (B, C) Pick<A, B, C, D>((A, B, C, D) t, ISig<B, C> _) => (t.Item2, t.Item3);
        public static (B, D) Pick<A, B, C, D>((A, B, C, D) t, ISig<B, D> _) => (t.Item2, t.Item4);
        public static (C, D) Pick<A, B, C, D>((A, B, C, D) t, ISig<C, D> _) => (t.Item3, t.Item4);
        public static (A, B, C) Pick<A, B, C, D>((A, B, C, D) t, ISig<A, B, C> _) => (t.Item1, t.Item2, t.Item3);
        public static (A, B, D) Pick<A, B, C, D>((A, B, C, D) t, ISig<A, B, D> _) => (t.Item1, t.Item2, t.Item4);
        public static (A, C, D) Pick<A, B, C, D>((A, B, C, D) t, ISig<A, C, D> _) => (t.Item1, t.Item3, t.Item4);
        public static (B, C, D) Pick<A, B, C, D>((A, B, C, D) t, ISig<B, C, D> _) => (t.Item2, t.Item3, t.Item4);
        public static (A, B, C, D) Pick<A, B, C, D>((A, B, C, D) t, ISig<A, B, C, D> _) => t;

        public static A Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<A> _) => t.Item1;
        public static B Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<B> _) => t.Item2;
        public static C Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<C> _) => t.Item3;
        public static D Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<D> _) => t.Item4;
        public static E Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<E> _) => t.Item5;
        public static (A, B) Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<A, B> _) => (t.Item1, t.Item2);
        public static (A, C) Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<A, C> _) => (t.Item1, t.Item3);
        public static (A, D) Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<A, D> _) => (t.Item1, t.Item4);
        public static (A, E) Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<A, E> _) => (t.Item1, t.Item5);
        public static (B, C) Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<B, C> _) => (t.Item2, t.Item3);
        public static (B, D) Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<B, D> _) => (t.Item2, t.Item4);
        public static (B, E) Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<B, E> _) => (t.Item2, t.Item5);
        public static (C, D) Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<C, D> _) => (t.Item3, t.Item4);
        public static (C, E) Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<C, E> _) => (t.Item3, t.Item5);
        public static (D, E) Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<D, E> _) => (t.Item4, t.Item5);
        public static (A, B, C) Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<A, B, C> _) => (t.Item1, t.Item2, t.Item3);
        public static (A, B, D) Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<A, B, D> _) => (t.Item1, t.Item2, t.Item4);
        public static (A, B, E) Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<A, B, E> _) => (t.Item1, t.Item2, t.Item5);
        public static (A, C, D) Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<A, C, D> _) => (t.Item1, t.Item3, t.Item4);
        public static (A, C, E) Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<A, C, E> _) => (t.Item1, t.Item3, t.Item5);
        public static (B, C, D) Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<B, C, D> _) => (t.Item2, t.Item3, t.Item4);
        public static (B, C, E) Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<B, C, E> _) => (t.Item2, t.Item3, t.Item5);
        public static (C, D, E) Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<C, D, E> _) => (t.Item3, t.Item4, t.Item5);
        public static (A, B, C, D) Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<A, B, C, D> _) => (t.Item1, t.Item2, t.Item3, t.Item4);
        public static (A, B, C, E) Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<A, B, C, E> _) => (t.Item1, t.Item2, t.Item3, t.Item5);
        public static (A, B, D, E) Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<A, B, D, E> _) => (t.Item1, t.Item2, t.Item4, t.Item5);
        public static (A, C, D, E) Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<A, C, D, E> _) => (t.Item1, t.Item3, t.Item4, t.Item5);
        public static (B, C, D, E) Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<B, C, D, E> _) => (t.Item2, t.Item3, t.Item4, t.Item5);
        public static (A, B, C, D, E) Pick<A, B, C, D, E>((A, B, C, D, E) t, ISig<A, B, C, D, E> _) => t;
    }
}
