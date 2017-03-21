using static KitchenSink.Operators;

namespace KitchenSink
{
    public interface IUntypedHeterogenousList
    {
        object UntypedHead { get; }
        object UntypedTail { get; }
    }

    public interface IHeterogenousList<out A, out B> : IUntypedHeterogenousList where B : IUntypedHeterogenousList
    {
        A Head { get; }
        B Tail { get; }
        IHeterogenousList<C, IHeterogenousList<A, B>> Cons<C>(C x);
        bool Contains<C>(C x);
    }

    public interface IEmptyHeterogenousList : IHeterogenousList<Void, IEmptyHeterogenousList>
    {
    }

    public static class HList
    {
        private sealed class Node<A, B> : IHeterogenousList<A, B> where B : IUntypedHeterogenousList
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
            public IHeterogenousList<C, IHeterogenousList<A, B>> Cons<C>(C x) =>
                new Node<C, IHeterogenousList<A, B>>(x, this);
            public bool Contains<C>(C x) => Equals(Head, x) || Dyn(Tail).Contains(x);
        }

        private sealed class VoidNode : IEmptyHeterogenousList
        {
            private VoidNode() { }
            public static readonly VoidNode It = new VoidNode();
            public Void Head => Void.It;
            public IEmptyHeterogenousList Tail => this;
            public object UntypedHead => Head;
            public object UntypedTail => Tail;
            public IHeterogenousList<C, IHeterogenousList<Void, IEmptyHeterogenousList>> Cons<C>(C x) =>
                Singleton(x);
            public bool Contains<C>(C x) => false;
        }

        public static IEmptyHeterogenousList Empty => VoidNode.It;

        public static IHeterogenousList<A, IEmptyHeterogenousList> Singleton<A>(A x) =>
            new Node<A, VoidNode>(x, VoidNode.It);

        public static IHeterogenousList<C, IHeterogenousList<A, B>> Cons<A, B, C>(
            C x,
            IHeterogenousList<A, B> hlist)
            where B : IUntypedHeterogenousList =>
            hlist.Cons(x);

        public static bool IsEmpty(this IEmptyHeterogenousList hlist) => true;
        public static bool IsEmpty<A, B>(this IHeterogenousList<A, B> hlist) where B : IUntypedHeterogenousList => false;

        public static int Length(this IEmptyHeterogenousList hlist) => 0;
        public static int Length<A, B>(this IHeterogenousList<A, B> hlist) where B : IUntypedHeterogenousList
            => 1 + Length(Dyn(hlist.Tail));
    }
}
