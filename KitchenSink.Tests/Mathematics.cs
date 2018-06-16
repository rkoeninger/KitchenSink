using System.Linq;
using KitchenSink.Extensions;
using static KitchenSink.Operators;
using KitchenSink.Testing;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    public class Mathematics
    {
        [Test]
        public void Divisibility()
        {
            Check.That(Sample.Ints, x => x.IsEven() == ((x / 2) * 2 == x));
            Check.That(Sample.Ints, x => x.IsOdd() == ((x / 2) * 2 != x));
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
            Assert.AreEqual(1, 0.Permutations(0));
            Assert.AreEqual(1, 1.Permutations(1));
            Assert.AreEqual(2, 2.Permutations(1));
            Assert.AreEqual(12, 4.Permutations(2));
            Assert.AreEqual(10 * 9 * 8 * 7, 10.Permutations(4));
            Expect.Error(() => (-1).Permutations(1));
            Expect.Error(() => 2.Permutations(3));
            Expect.Error(() => 4.Permutations(-1));
        }

        [Test]
        public void Combinations()
        {
            Assert.AreEqual(1, 0.Combinations(0));
            Assert.AreEqual(1, 1.Combinations(1));
            Assert.AreEqual(2, 2.Combinations(1));
            Assert.AreEqual(6, 4.Combinations(2));
            Assert.AreEqual((10 * 9 * 8 * 7) / (4 * 3 * 2), 10.Combinations(4));
            Expect.Error(() => (-1).Combinations(1));
            Expect.Error(() => 2.Combinations(3));
            Expect.Error(() => 4.Combinations(-1));
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

            Assert.AreEqual(seq1.Count.Combinations(subsetSize), combinations.Count);
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

            Assert.AreEqual(seq1.Count.Permutations(subsetSize), permutations.Count);
            Assert.AreEqual(expectedPermutations.Count, permutations.Count);

            foreach (var expected in expectedPermutations)
            {
                Assert.IsTrue(permutations.Any(x => x.SequenceEqual(expected)));
            }
        }
    }
}
