using System;
using System.Linq;
using KitchenSink.Extensions;
using static KitchenSink.Operators;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    public class LazySequences
    {
        [Test]
        public void OverlappingPairsTest()
        {
            Assert.IsEmpty(SeqOf<int>().OverlappingPairs());
            Assert.Throws<ArgumentException>(() =>
            {
                var _ = SeqOf(1).OverlappingPairs().ToArray();
            });
            Assert.AreEqual(
                SeqOf((1, 2), (2, 3), (3, 4), (4, 5)),
                SeqOf(1, 2, 3, 4, 5).OverlappingPairs());
        }

        [Test]
        public void DealTest()
        {
            var seq = ListOf(1, 2, 3, 4, 5, 6, 7, 8, 9);
            Assert.AreEqual(SeqOf(1, 4, 7, 2, 5, 8, 3, 6, 9), seq.Deal(3).Flatten());
            Assert.AreEqual(SeqOf(2, 4, 6, 8), seq.Deal(2).ElementAt(1));
        }

        [Test]
        public void CrossProduct()
        {
            var xs = SeqOf(1, 2, 3);
            var ys = SeqOf(4, 5, 6);
            Assert.AreEqual(
                SeqOf(5, 6, 7, 6, 7, 8, 7, 8, 9),
                xs.CrossJoin(ys, Add));
        }
    }
}
