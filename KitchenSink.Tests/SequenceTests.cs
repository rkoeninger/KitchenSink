using System;
using System.Linq;
using KitchenSink.Collections;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    [TestFixture]
    public class SequenceTests
    {
        [Test]
        public void ListIteration()
        {
            var list = AList.Of(1, 2, 3, 4, 5, 6, 7, 8);
            var seq = Sequence.Of(1, 2, 3, 4, 5, 6, 7, 8);
            Assert.IsTrue(list.SequenceEqual(seq));
        }

        [Test]
        public void EnumerableIteration()
        {
            var list = AList.Of(1, 2, 3, 4, 5, 6, 7, 8);
            var seq = new [] {1, 2, 3, 4, 5, 6, 7, 8}.ToSequence();
            Assert.IsTrue(list.SequenceEqual(seq));
        }

        [Test]
        public void OverlappingPartition()
        {
            var list = AList.Of(1, 2, 3, 4, 5);
            var expected = AList.Of(
                Tuple.Create(1, 2),
                Tuple.Create(2, 3),
                Tuple.Create(3, 4),
                Tuple.Create(4, 5));
            Assert.IsTrue(expected.SequenceEqual(list.OverlappingPartition2()));
        }
    }
}
