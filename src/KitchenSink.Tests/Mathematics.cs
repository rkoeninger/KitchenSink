using System.Collections.Generic;
using System.Linq;
using KitchenSink.Extensions;
using KitchenSink.Testing;
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
        public void Subfactorial()
        {
            Assert.AreEqual(1, 0.Subfactorial());
            Assert.AreEqual(0, 1.Subfactorial());
            Assert.AreEqual(1, 2.Subfactorial());
            Assert.AreEqual(2, 3.Subfactorial());
            Assert.AreEqual(9, 4.Subfactorial());
            Expect.Error(() => (-1).Subfactorial());
            Expect.Error(() => (-2).Subfactorial());
        }

        [Test]
        public void PermutationCount()
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
        public void CombinationCount()
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
        public void CombinationCountMirrors([Range(4, 12)] int n)
        {
            foreach (var r in Enumerable.Range(0, n / 2))
            {
                Assert.AreEqual(n.CombinationCount(r), n.CombinationCount(n - r));
            }
        }

        [Test]
        public void Combinations()
        {
            var seq1 = ListOf(5, 6, 7, 8, 9);
            const int subsetSize = 3;
            var combinations = seq1.Combinations(subsetSize)
                .Select(xs => xs.ToList())
                .ToList();
            var expectedCombinations = ListOf(
                ListOf(5, 6, 7),
                ListOf(5, 6, 8),
                ListOf(5, 6, 9),
                ListOf(5, 7, 8),
                ListOf(5, 7, 9),
                ListOf(5, 8, 9),
                ListOf(6, 7, 8),
                ListOf(6, 7, 9),
                ListOf(6, 8, 9),
                ListOf(7, 8, 9));

            Assert.AreEqual(seq1.Count.CombinationCount(subsetSize), combinations.Count);
            Assert.IsTrue(combinations.All(xs => xs.Count == subsetSize));
            Assert.AreEqual(expectedCombinations.Count, combinations.Count);

            foreach (var expected in expectedCombinations)
            {
                Assert.IsTrue(combinations.Any(xs => expected.All(y => xs.Contains(y))));
            }
        }

        [Test]
        public void Permutations()
        {
            var seq1 = ListOf(5, 6, 7, 8, 9);
            const int subsetSize = 2;
            var permutations = seq1.Permutations(subsetSize).ToList();
            var expectedPermutations = ListOf(
                ListOf(5, 6),
                ListOf(5, 7),
                ListOf(5, 8),
                ListOf(5, 9),
                ListOf(6, 5),
                ListOf(6, 7),
                ListOf(6, 8),
                ListOf(6, 9),
                ListOf(7, 5),
                ListOf(7, 6),
                ListOf(7, 8),
                ListOf(7, 9),
                ListOf(8, 5),
                ListOf(8, 6),
                ListOf(8, 7),
                ListOf(8, 9),
                ListOf(9, 5),
                ListOf(9, 6),
                ListOf(9, 7),
                ListOf(9, 8));

            Assert.AreEqual(seq1.Count.PermutationCount(subsetSize), permutations.Count);
            Assert.AreEqual(expectedPermutations.Count, permutations.Count);

            foreach (var expected in expectedPermutations)
            {
                Assert.IsTrue(permutations.Any(x => x.SequenceEqual(expected)));
            }
        }

        [Test]
        [TestCaseSource(nameof(DerangementCases))]
        public void List(List<int> list, List<List<int>> expectedDerangements)
        {
            var derangements = list.Derangements().ToList();
            Assert.AreEqual(list.Count.DerangementCount(), derangements.Count);
            Assert.AreEqual(expectedDerangements.Count, derangements.Count);

            foreach (var expected in expectedDerangements)
            {
                Assert.IsTrue(derangements.Any(x => x.SequenceEqual(expected)));
            }
        }

        private static IEnumerable<TestCaseData> DerangementCases => SeqOf(
            new TestCaseData(
                ListOf<int>(),
                ListOf(
                    ListOf<int>())),
            new TestCaseData(
                ListOf(1),
                ListOf<List<int>>()),
            new TestCaseData(
                ListOf(1, 2),
                ListOf(
                    ListOf(2, 1))),
            new TestCaseData(
                ListOf(1, 2, 3),
                ListOf(
                    ListOf(2, 3, 1),
                    ListOf(3, 1, 2))),
            new TestCaseData(
                ListOf(1, 2, 3, 4),
                ListOf(
                    ListOf(2, 1, 4, 3),
                    ListOf(2, 3, 4, 1),
                    ListOf(2, 4, 1, 3),
                    ListOf(3, 1, 4, 2),
                    ListOf(3, 4, 1, 2),
                    ListOf(3, 4, 2, 1),
                    ListOf(4, 1, 2, 3),
                    ListOf(4, 3, 1, 2),
                    ListOf(4, 3, 2, 1))));
    }
}
