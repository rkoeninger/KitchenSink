using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class SequenceTests
    {
        [TestMethod]
        public void ListIteration()
        {
            var list = List.Of(1, 2, 3, 4, 5, 6, 7, 8);
            var seq = Sequence.Of(1, 2, 3, 4, 5, 6, 7, 8);
            Assert.IsTrue(list.SequenceEqual(seq));
        }

        [TestMethod]
        public void EnumerableIteration()
        {
            var list = List.Of(1, 2, 3, 4, 5, 6, 7, 8);
            var seq = new [] {1, 2, 3, 4, 5, 6, 7, 8}.ToSequence();
            Assert.IsTrue(list.SequenceEqual(seq));
        }
    }
}
