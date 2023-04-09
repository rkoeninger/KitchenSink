using System.Linq;
using KitchenSink.Extensions;
using static KitchenSink.Operators;
using NUnit.Framework;
using KitchenSink.Testing;

namespace KitchenSink.Tests
{
    public class LazySequences
    {
        [Test]
        public void ClumpTest()
        {
            Expect.That(0.To(4), size => !SeqOf<int>().Clump(size).Any());
            Expect.That(2.To(5), size =>
            {
                var seq = 1.To(10).ToArray();
                return seq.Clump(size).Count() == seq.Length - size + 1;
            });
            Assert.AreEqual(
                SeqOf(SeqOf(1, 2, 3), SeqOf(2, 3, 4), SeqOf(3, 4, 5)),
                SeqOf(1, 2, 3, 4, 5).Clump(3));
            Assert.AreEqual(
                SeqOf(SeqOf(1, 2), SeqOf(2, 3), SeqOf(3, 4), SeqOf(4, 5)),
                SeqOf(1, 2, 3, 4, 5).Clump(2));
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

        [Test]
        public void Transposing()
        {
            Assert.AreEqual(
                SeqOf(SeqOf(1, 4, 7), SeqOf(2, 5, 8), SeqOf(3, 6, 9)),
                SeqOf(SeqOf(1, 2, 3), SeqOf(4, 5, 6), SeqOf(7, 8, 9)).Transpose());
        }
    }
}
