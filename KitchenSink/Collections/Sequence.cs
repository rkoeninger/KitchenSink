using System;
using System.Collections;
using System.Collections.Generic;

namespace KitchenSink.Collections
{
    /// <summary>
    /// Like an IEnumerator, but persistent.
    /// Like a List, but lazily-evaluated.
    /// </summary>
    public interface ISequence<A> : IEnumerable<A>
    {
        /// <summary>
        /// The current value in this Sequence. Will be None if the Sequence is exhausted.
        /// </summary>
        Maybe<A> Current { get; }

        /// <summary>
        /// The next node in this Sequence. If this node is None, then the next is gauranteed to be.
        /// </summary>
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
            var list = values as IReadOnlyList<A>;

            return (list != null)
                ? (ISequence<A>) new ListSequence<A>(list)
                : new EnumeratorSequence<A>(values.GetEnumerator());
        }

        public static ISequence<A> Empty<A>()
        {
            return EmptySequence<A>.It;
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

    internal struct EmptySequence<A> : ISequence<A>
    {
        internal static ISequence<A> It = new EmptySequence<A>();

        public Maybe<A> Current => Maybe<A>.None;
        public ISequence<A> Next => this;

        public IEnumerator<A> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal static class EmptySequencePair<A>
    {
        internal static SequencePair<A> It = new SequencePair<A>(Maybe<A>.None, EmptySequence<A>.It);
    }

    internal struct EnumeratorSequence<A> : ISequence<A>
    {
        public EnumeratorSequence(IEnumerator<A> e)
        {
            LazyPair = new Lazy<SequencePair<A>>(() => e.MoveNext()
                ? new SequencePair<A>(Maybe.Some(e.Current), new EnumeratorSequence<A>(e))
                : EmptySequencePair<A>.It);
        }

        private readonly Lazy<SequencePair<A>> LazyPair;

        public Maybe<A> Current => LazyPair.Value.Current;
        public ISequence<A> Next => LazyPair.Value.Next;

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

        public Maybe<A> Current => List[Index];
        public ISequence<A> Next => LazyNext.Value;

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
