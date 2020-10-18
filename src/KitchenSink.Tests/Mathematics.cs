using System.Linq;
using KitchenSink.Extensions;
using static KitchenSink.Operators;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    public class Mathematics
    {
        [Test]
        public void Divisibility()
        {
            Expect.That(Sample.Ints, x => x.IsEven() == ((x / 2) * 2 == x));
            Expect.That(Sample.Ints, x => x.IsOdd() == ((x / 2) * 2 != x));
        }

        [Test]
        public void Factorial()
        {
            Assert.AreEqual(1, 0.Factorial());
            Assert.AreEqual(1, 1.Factorial());
            Assert.AreEqual(2, 2.Factorial());
            Assert.AreEqual(6, 3.Factorial());
            Assert.AreEqual(24, 4.Factorial());
            Expect.Error(() => (-1).Factorial());
            Expect.Error(() => (-2).Factorial());
        }

        [Test]
        public void Permutations()
        {
            Assert.AreEqual(1, 0.PermutationCount(0));
            Assert.AreEqual(1, 1.PermutationCount(1));
            Assert.AreEqual(2, 2.PermutationCount(1));
            Assert.AreEqual(12, 4.PermutationCount(2));
            Assert.AreEqual(10 * 9 * 8 * 7, 10.PermutationCount(4));
            Expect.Error(() => (-1).PermutationCount(1));
            Expect.Error(() => 2.PermutationCount(3));
            Expect.Error(() => 4.PermutationCount(-1));
        }

        [Test]
        public void Combinations()
        {
            Assert.AreEqual(1, 0.CombinationCount(0));
            Assert.AreEqual(1, 1.CombinationCount(1));
            Assert.AreEqual(2, 2.CombinationCount(1));
            Assert.AreEqual(6, 4.CombinationCount(2));
            Assert.AreEqual((10 * 9 * 8 * 7) / (4 * 3 * 2), 10.CombinationCount(4));
            Expect.Error(() => (-1).CombinationCount(1));
            Expect.Error(() => 2.CombinationCount(3));
            Expect.Error(() => 4.CombinationCount(-1));
        }

        [Test]
        public void CombinationsOfSet()
        {
            var seq1 = ListOf(5, 6, 7, 8, 9);
            const int subsetSize = 3;
            var combinations = seq1.Combinations(subsetSize).ToList();
            var expectedCombinations = ListOf(
                SeqOf(5, 6, 7),
                SeqOf(5, 6, 8),
                SeqOf(5, 6, 9),
                SeqOf(5, 7, 8),
                SeqOf(5, 7, 9),
                SeqOf(5, 8, 9),
                SeqOf(6, 7, 8),
                SeqOf(6, 7, 9),
                SeqOf(6, 8, 9),
                SeqOf(7, 8, 9)
            );

            Assert.AreEqual(seq1.Count.CombinationCount(subsetSize), combinations.Count);
            Assert.AreEqual(expectedCombinations.Count, combinations.Count);

            foreach (var expected in expectedCombinations)
            {
                Assert.IsTrue(combinations.Any(x => x.SequenceEqual(expected)));
            }
        }

        [Test]
        public void PermutationsOfSet()
        {
            var seq1 = ListOf(5, 6, 7, 8, 9);
            const int subsetSize = 2;
            var permutations = seq1.Permutations(subsetSize).ToList();
            var expectedPermutations = ListOf(
                SeqOf(5, 6),
                SeqOf(5, 7),
                SeqOf(5, 8),
                SeqOf(5, 9),
                SeqOf(6, 5),
                SeqOf(6, 7),
                SeqOf(6, 8),
                SeqOf(6, 9),
                SeqOf(7, 5),
                SeqOf(7, 6),
                SeqOf(7, 8),
                SeqOf(7, 9),
                SeqOf(8, 5),
                SeqOf(8, 6),
                SeqOf(8, 7),
                SeqOf(8, 9),
                SeqOf(9, 5),
                SeqOf(9, 6),
                SeqOf(9, 7),
                SeqOf(9, 8)
            );

            Assert.AreEqual(seq1.Count.PermutationCount(subsetSize), permutations.Count);
            Assert.AreEqual(expectedPermutations.Count, permutations.Count);

            foreach (var expected in expectedPermutations)
            {
                Assert.IsTrue(permutations.Any(x => x.SequenceEqual(expected)));
            }
        }
    }
}
