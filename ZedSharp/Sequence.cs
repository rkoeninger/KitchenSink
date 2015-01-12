using System;
using System.Collections;
using System.Collections.Generic;

namespace ZedSharp
{
    public interface ISequence<A> : IEnumerable<A>
    {
        Maybe<A> Current { get; }

        ISequence<A> Next { get; }
    }

    public static class Sequence
    {
        public static ISequence<A> Of<A>(params A[] values)
        {
            return new ListSequence<A>(values);
        }

        public static ISequence<A> ToSequence<A>(this IEnumerable<A> values)
        {
            return new EnumeratorSequence<A>(values.GetEnumerator());
        }

        public static ISequence<A> Empty<A>()
        {
            return EmptySequence<A>.It;
        }
    }

    internal struct EmptySequence<A> : ISequence<A>
    {
        internal static ISequence<A> It = new EmptySequence<A>();

        public Maybe<A> Current
        {
            get { return Maybe<A>.None; }
        }

        public ISequence<A> Next
        {
            get { return this; }
        }

        public IEnumerator<A> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal struct EnumeratorSequence<A> : ISequence<A>
    {
        public EnumeratorSequence(IEnumerator<A> e)
        {
            LazyPair = new Lazy<SequencePair<A>>(() => e.MoveNext()
                ? new SequencePair<A>(Maybe.Some(e.Current), new EnumeratorSequence<A>(e))
                : new SequencePair<A>(Maybe<A>.None, EmptySequence<A>.It));
        }

        private readonly Lazy<SequencePair<A>> LazyPair;

        public Maybe<A> Current
        {
            get { return LazyPair.Value.Current; }
        }

        public ISequence<A> Next
        {
            get { return LazyPair.Value.Next; }
        }

        public IEnumerator<A> GetEnumerator()
        {
            ISequence<A> seq = this;

            while (seq.Current.HasValue)
            {
                yield return seq.Current.Value;
                seq = seq.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal struct SequencePair<A>
    {
        public SequencePair(Maybe<A> current, ISequence<A> next)
        {
            Current = current;
            Next = next;
        }

        internal Maybe<A> Current;
        internal ISequence<A> Next;
    }

    internal struct ListSequence<A> : ISequence<A>
    {
        public ListSequence(IReadOnlyList<A> list, int index = 0)
        {
            List = list;
            Index = index;
            LazyNext = new Lazy<ISequence<A>>(() => new ListSequence<A>(list, index + 1));
        }

        private readonly IReadOnlyList<A> List;
        private readonly int Index;
        private readonly Lazy<ISequence<A>> LazyNext;

        public Maybe<A> Current
        {
            get { return List[Index]; }
        }

        public ISequence<A> Next
        {
            get { return LazyNext.Value; }
        }

        public IEnumerator<A> GetEnumerator()
        {
            for (var i = Index; i < List.Count; ++i)
                yield return List[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
