using static KitchenSink.Operators;

namespace KitchenSink
{
    public interface IUntypedHeterogeneousList
    {
        object UntypedHead { get; }
        object UntypedTail { get; }
    }

    public interface IHeterogeneousList<out A, out B> : IUntypedHeterogeneousList where B : IUntypedHeterogeneousList
    {
        A Head { get; }
        B Tail { get; }
        IHeterogeneousList<C, IHeterogeneousList<A, B>> Cons<C>(C x);
        bool Contains<C>(C x);
    }

    public interface IEmptyHeterogeneousList : IHeterogeneousList<Void, IEmptyHeterogeneousList>
    {
    }

    public static class HList
    {
        private sealed class Node<A, B> : IHeterogeneousList<A, B> where B : IUntypedHeterogeneousList
        {
            public Node(A head, B tail)
            {
                Head = head;
                Tail = tail;
            }

            public A Head { get; }
            public B Tail { get; }
            public object UntypedHead => Head;
            public object UntypedTail => Tail;
            public IHeterogeneousList<C, IHeterogeneousList<A, B>> Cons<C>(C x) =>
                new Node<C, IHeterogeneousList<A, B>>(x, this);
            public bool Contains<C>(C x) => Equals(Head, x) || Dyn(Tail).Contains(x);
        }

        private sealed class VoidNode : IEmptyHeterogeneousList
        {
            private VoidNode() { }
            public static readonly VoidNode It = new VoidNode();
            public Void Head => Void.It;
            public IEmptyHeterogeneousList Tail => this;
            public object UntypedHead => Head;
            public object UntypedTail => Tail;
            public IHeterogeneousList<C, IHeterogeneousList<Void, IEmptyHeterogeneousList>> Cons<C>(C x) =>
                Singleton(x);
            public bool Contains<C>(C x) => false;
        }

        public static IEmptyHeterogeneousList Empty => VoidNode.It;

        public static IHeterogeneousList<A, IEmptyHeterogeneousList> Singleton<A>(A x) =>
            new Node<A, VoidNode>(x, VoidNode.It);

        public static IHeterogeneousList<C, IHeterogeneousList<A, B>> Cons<A, B, C>(
            C x,
            IHeterogeneousList<A, B> hlist)
            where B : IUntypedHeterogeneousList =>
            hlist.Cons(x);

        public static bool IsEmpty(this IEmptyHeterogeneousList hlist) => true;
        public static bool IsEmpty<A, B>(this IHeterogeneousList<A, B> hlist) where B : IUntypedHeterogeneousList => false;

        public static int Length(this IEmptyHeterogeneousList hlist) => 0;
        public static int Length<A, B>(this IHeterogeneousList<A, B> hlist) where B : IUntypedHeterogeneousList
            => 1 + Length(Dyn(hlist.Tail));
    }
}
