using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KitchenSink.Extensions;
using static KitchenSink.Operators;

namespace KitchenSink.Collections
{
    public static class FingerTree
    {
        public static IFingerTree<A> Empty<A>() => new EmptyFingerTree<A>();
        public static IFingerTree<A> ToFingerTree<A>(this IEnumerable<A> seq) =>
            seq.Aggregate(Empty<A>(), (tree, x) => tree.EnqueueSuffix(x));
        public static IFingerTree<A> ToFingerTree<A>(params A[] elements) =>
            ToFingerTree(elements.AsEnumerable());
    }

    public interface IFingerTree<A> : IEnumerable<A>
    {
        bool IsEmpty { get; }
        IFingerTree<A> EnqueuePrefix(A value);
        IFingerTree<A> EnqueueSuffix(A value);
        (IFingerTree<A>, Maybe<A>) DequeuePrefix();
        (IFingerTree<A>, Maybe<A>) DequeueSuffix();
        Maybe<A> CurrentPrefix { get; }
        Maybe<A> CurrentSuffix { get; }
    }

    internal class DeepFingerTree<A> : IFingerTree<A>
    {
        private readonly A[] prefix;
        private readonly A[] suffix;
        private readonly IFingerTree<A[]> tree;

        internal DeepFingerTree(A[] prefix, A[] suffix, IFingerTree<A[]> tree)
        {
            this.prefix = prefix;
            this.suffix = suffix;
            this.tree = tree;
        }

        public bool IsEmpty => false;
        public IFingerTree<A> EnqueuePrefix(A value) =>
            prefix.Length < 4
                ? new DeepFingerTree<A>(
                    ArrayOf(value).Concat(prefix),
                    suffix,
                    tree)
                : new DeepFingerTree<A>(
                    ArrayOf(value, prefix.First()),
                    suffix,
                    tree.EnqueuePrefix(prefix.Skip(1).ToArray()));
        public IFingerTree<A> EnqueueSuffix(A value) =>
            suffix.Length < 4
                ? new DeepFingerTree<A>(
                    prefix,
                    suffix.Concat(ArrayOf(value)),
                    tree)
                : new DeepFingerTree<A>(
                    prefix,
                    ArrayOf(suffix.Last(), value),
                    tree.EnqueueSuffix(suffix.Take(suffix.Length - 1).ToArray()));
        public (IFingerTree<A>, Maybe<A>) DequeuePrefix() =>
            (prefix.Length > 1
                ? new DeepFingerTree<A>(
                    prefix.Skip(1).ToArray(),
                    suffix,
                    tree)
                : SplitTreeLeft(),
             CurrentPrefix);
        public (IFingerTree<A>, Maybe<A>) DequeueSuffix() =>
            (suffix.Length > 1
                ? new DeepFingerTree<A>(
                    prefix,
                    suffix.Take(suffix.Length - 1).ToArray(),
                    tree)
                : SplitTreeRight(),
             CurrentSuffix);
        public Maybe<A> CurrentPrefix => prefix.First();
        public Maybe<A> CurrentSuffix => suffix.Last();

        public IEnumerator<A> GetEnumerator() =>
            prefix.Concat(tree.SelectMany(x => x)).Concat(suffix).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        private IFingerTree<A> SplitTreeLeft()
        {
            if (tree is SingleFingerTree<A[]>)
            {
                return (((SingleFingerTree<A>)tree).Concat(suffix)).ToFingerTree();
            }

            var (newTree, newPrefix) = tree.DequeuePrefix();
            return newPrefix.Branch(
                p => new DeepFingerTree<A>(p, suffix, newTree),
                () => suffix.ToFingerTree());
        }
        
        private IFingerTree<A> SplitTreeRight()
        {
            if (tree is SingleFingerTree<A[]>)
            {
                return prefix.Concat((SingleFingerTree<A>)tree).ToFingerTree();
            }

            var (newTree, newSuffix) = tree.DequeueSuffix();
            return newSuffix.Branch(
                s => new DeepFingerTree<A>(prefix, s, newTree),
                () => prefix.ToFingerTree());
        }
    }

    internal class SingleFingerTree<A> : IFingerTree<A>
    {
        private readonly A value;

        internal SingleFingerTree(A value)
        {
            this.value = value;
        }

        public bool IsEmpty => false;
        public IFingerTree<A> EnqueuePrefix(A value) =>
            new DeepFingerTree<A>(
                ArrayOf(value),
                ArrayOf(this.value),
                FingerTree.Empty<A[]>());
        public IFingerTree<A> EnqueueSuffix(A value) =>
            new DeepFingerTree<A>(
                ArrayOf(this.value),
                ArrayOf(value),
                FingerTree.Empty<A[]>());
        public (IFingerTree<A>, Maybe<A>) DequeuePrefix() => (FingerTree.Empty<A>(), value);
        public (IFingerTree<A>, Maybe<A>) DequeueSuffix() => (FingerTree.Empty<A>(), value);
        public Maybe<A> CurrentPrefix => value;
        public Maybe<A> CurrentSuffix => value;

        public IEnumerator<A> GetEnumerator()
        {
            yield return value;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    internal class EmptyFingerTree<A> : IFingerTree<A>
    {
        internal EmptyFingerTree()
        {
        }

        public bool IsEmpty => true;
        public IFingerTree<A> EnqueuePrefix(A value) => new SingleFingerTree<A>(value);
        public IFingerTree<A> EnqueueSuffix(A value) => new SingleFingerTree<A>(value);
        public (IFingerTree<A>, Maybe<A>) DequeuePrefix() => (this, None<A>());
        public (IFingerTree<A>, Maybe<A>) DequeueSuffix() => (this, None<A>());
        public Maybe<A> CurrentPrefix => None<A>();
        public Maybe<A> CurrentSuffix => None<A>();

        public IEnumerator<A> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
