using System.Linq;
using KitchenSink.Collections;
using static KitchenSink.Collections.ConstructionOperators;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    [TestFixture]
    public class SequenceTests
    {
        [Test]
        public void ListIteration()
        {
            var list = listof(1, 2, 3, 4, 5, 6, 7, 8);
            var seq = Sequence.Of(1, 2, 3, 4, 5, 6, 7, 8);
            Assert.IsTrue(list.SequenceEqual(seq));
        }

        [Test]
        public void EnumerableIteration()
        {
            var list = listof(1, 2, 3, 4, 5, 6, 7, 8);
            var seq = new [] {1, 2, 3, 4, 5, 6, 7, 8}.ToSequence();
            Assert.IsTrue(list.SequenceEqual(seq));
        }

        [Test]
        public void OverlappingPartition()
        {
            var list = listof(1, 2, 3, 4, 5);
            var expected = listof(
                tupleof(1, 2),
                tupleof(2, 3),
                tupleof(3, 4),
                tupleof(4, 5));
            Assert.IsTrue(expected.SequenceEqual(list.OverlappingPartition2()));
        }
    }
}
