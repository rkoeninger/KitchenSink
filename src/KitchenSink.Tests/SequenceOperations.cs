using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using KitchenSink.Extensions;
using KitchenSink.Testing;
using static KitchenSink.Operators;

namespace KitchenSink.Tests
{
    public class SequenceOperations
    {
        [Test]
        public void InterleavingEqualLength()
        {
            var xs = SeqOf(1, 2, 3);
            var ys = SeqOf(6, 7, 8);
            Assert.IsTrue(SeqOf(1, 6, 2, 7, 3, 8).SequenceEqual(xs.Interleave(ys)));
        }

        [Test]
        public void InterleavingEqualFirstIsShorter()
        {
            var xs = SeqOf(1, 2);
            var ys = SeqOf(6, 7, 8, 9);
            Assert.IsTrue(SeqOf(1, 6, 2, 7, 8, 9).SequenceEqual(xs.Interleave(ys)));
        }

        [Test]
        public void InterleavingEqualSecondIsShorter()
        {
            var xs = SeqOf(1, 2, 3, 4);
            var ys = SeqOf(6, 7);
            Assert.IsTrue(SeqOf(1, 6, 2, 7, 3, 4).SequenceEqual(xs.Interleave(ys)));
        }

        [Test]
        public void InterleavingSeveralSequences()
        {
            var xs = SeqOf(1, 2, 3, 4);
            var ys = SeqOf(5, 6, 7, 8);
            var zs = SeqOf(9, 10, 11, 12);
            var ws = SeqOf(13, 14, 15, 16);
            var expected = SeqOf(1, 5, 9, 13, 2, 6, 10, 14, 3, 7, 11, 15, 4, 8, 12, 16);
            Assert.IsTrue(expected.SequenceEqual(xs.Interleave(ys, zs, ws)));
        }

        [Test]
        public void IntersperseShortSequence()
        {
            var xs = SeqOf(1, 2, 3);
            Assert.IsTrue(SeqOf(1, 0, 2, 0, 3).SequenceEqual(xs.Intersperse(0)));
        }

        [Test]
        public void IntersperseSingleElement()
        {
            var xs = SeqOf(1);
            Assert.IsTrue(SeqOf(1).SequenceEqual(xs.Intersperse(0)));
        }

        [Test]
        public void IntersperseEmptySequence()
        {
            var xs = SeqOf<int>();
            Assert.IsTrue(SeqOf<int>().SequenceEqual(xs.Intersperse(0)));
        }

        [Test]
        public void IntersperseManyShortSequence()
        {
            var xs = SeqOf(1, 2, 3);
            Assert.IsTrue(SeqOf(1, 8, 9, 2, 8, 9, 3).SequenceEqual(xs.Intersperse(SeqOf(8, 9))));
        }

        [Test]
        public void EnumerableAsFunc()
        {
            var xs = SeqOf(1, 2, 3);
            var f = xs.AsFunc();
            Expect.IsSome(1, f());
            Expect.IsSome(2, f());
            Expect.IsSome(3, f());
            Expect.IsNone(f());
            Expect.IsNone(f()); // Should return None forever
        }

        [Test]
        public void FlattenSequenceOfVariableDepth()
        {
            var xs =
                SeqOf<object>(
                    SeqOf<object>(1, SeqOf<object>(2, 3)),
                    SeqOf<object>(SeqOf<object>(4), 5),
                    SeqOf<object>(SeqOf<object>(6, 7), 8));
            var ys = xs
                .Flatten(x =>
                    Either.If(
                        () => x is int,
                        () => (int)x,
                        () => (IEnumerable<object>) x));
            Assert.IsTrue(SeqOf(1, 2, 3, 4, 5, 6, 7, 8).SequenceEqual(ys));
        }
    }
}
