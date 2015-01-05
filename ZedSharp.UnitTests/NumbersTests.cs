using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class NumbersTests
    {
        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        public void CombinationsOfSet()
        {
            var seq1 = Blob.Of(5, 6, 7, 8, 9);
            const int subsetSize = 3;
            var combinations = seq1.Combinations(subsetSize).ToBlob();
            var expectedCombinations = Blob.Of(
                Seq.Of(5,6,7),
                Seq.Of(5,6,8),
                Seq.Of(5,6,9),
                Seq.Of(5,7,8),
                Seq.Of(5,7,9),
                Seq.Of(5,8,9),
                Seq.Of(6,7,8),
                Seq.Of(6,7,9),
                Seq.Of(6,8,9),
                Seq.Of(7,8,9)
            );

            Assert.AreEqual(seq1.Count().Combinations(subsetSize), combinations.Count());
            Assert.AreEqual(expectedCombinations.Count(), combinations.Count());

            foreach (var expected in expectedCombinations)
            {
                Assert.IsTrue(combinations.Any(x => x.SequenceEqual(expected)));
            }
        }

        [TestMethod]
        public void PermutationsOfSet()
        {
            var seq1 = Blob.Of(5, 6, 7, 8, 9);
            const int subsetSize = 2;
            var permutations = seq1.Permutations(subsetSize).ToBlob();
            var expectedPermutations = Blob.Of(
                Seq.Of(5, 6),
                Seq.Of(5, 7),
                Seq.Of(5, 8),
                Seq.Of(5, 9),
                Seq.Of(6, 5),
                Seq.Of(6, 7),
                Seq.Of(6, 8),
                Seq.Of(6, 9),
                Seq.Of(7, 5),
                Seq.Of(7, 6),
                Seq.Of(7, 8),
                Seq.Of(7, 9),
                Seq.Of(8, 5),
                Seq.Of(8, 6),
                Seq.Of(8, 7),
                Seq.Of(8, 9),
                Seq.Of(9, 5),
                Seq.Of(9, 6),
                Seq.Of(9, 7),
                Seq.Of(9, 8)
            );

            Assert.AreEqual(seq1.Count().Permutations(subsetSize), permutations.Count());
            Assert.AreEqual(expectedPermutations.Count(), permutations.Count());

            foreach (var expected in expectedPermutations)
            {
                Assert.IsTrue(permutations.Any(x => x.SequenceEqual(expected)));
            }
        }
    }
}
