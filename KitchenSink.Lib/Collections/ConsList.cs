using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static KitchenSink.Operators;

namespace KitchenSink.Collections
{
    public static class ConsList
    {
        public static IConsList<A> Empty<A>() => EmptyList<A>.It;

        public static void Branch<A>(this IConsList<A> list, Action<A> ifItem, Action ifEmpty) =>
            list.HeadMaybe.Branch(ifItem, ifEmpty);

        public static B Branch<A, B>(this IConsList<A> list, Func<A, B> ifItem, Func<B> ifEmpty) =>
            list.HeadMaybe.Branch(ifItem, ifEmpty);

        public static IConsList<A> InReverse<A>(this IConsList<A> list) =>
            list.Aggregate(Empty<A>(), (result, x) => result.Cons(x));
    }

    public interface IConsList<A> : IEnumerable<A>
    {
        bool IsEmpty { get; }
        int Count { get; }
        A Head { get; }
        IConsList<A> Tail { get; }
        Maybe<A> HeadMaybe { get; }
        Maybe<IConsList<A>> TailMaybe { get; }
        IConsList<A> Cons(A value);
    }

    public class ConsList<A> : IConsList<A>
    {
        public ConsList(A head, IConsList<A> tail)
        {
            Head = head;
            Tail = tail;
            Count = tail.Count + 1;
        }

        public bool IsEmpty => false;
        public int Count { get; }
        public A Head { get; }
        public IConsList<A> Tail { get; }
        public Maybe<A> HeadMaybe => Some(Head);
        public Maybe<IConsList<A>> TailMaybe => Some(Tail);
        public IConsList<A> Cons(A value) => new ConsList<A>(value, this);

        public IEnumerator<A> GetEnumerator()
        {
            IConsList<A> current = this;

            while (!current.IsEmpty)
            {
                yield return current.Head;
                current = current.Tail;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class EmptyList<A> : IConsList<A>
    {
        public static readonly IConsList<A> It = new EmptyList<A>();

        public bool IsEmpty => true;
        public int Count => 0;
        public A Head => throw new Exception();
        public IConsList<A> Tail => throw new Exception();
        public Maybe<A> HeadMaybe => None<A>();
        public Maybe<IConsList<A>> TailMaybe => None<IConsList<A>>();
        public IConsList<A> Cons(A value) => new ConsList<A>(value, this);

        public IEnumerator<A> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
