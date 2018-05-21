using System;
using System.Linq;
using KitchenSink.Extensions;
using static KitchenSink.Operators;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    public class EnumerableTests
    {
        [Test]
        public void OverlappingPairsTest()
        {
            Assert.IsEmpty(SeqOf<int>().OverlappingPairs());
            Assert.Throws<ArgumentException>(() =>
            {
                var _ = SeqOf(1).OverlappingPairs().ToArray();
            });
            Assert.IsTrue(
                SeqOf(
                    TupleOf(1, 2),
                    TupleOf(2, 3),
                    TupleOf(3, 4),
                    TupleOf(4, 5)).SequenceEqual(
                    SeqOf(1, 2, 3, 4, 5).OverlappingPairs()));
        }
    }
}
