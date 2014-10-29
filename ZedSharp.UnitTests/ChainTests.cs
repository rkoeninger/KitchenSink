using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class ChainTests
    {
        [TestMethod]
        public void ChainEquality()
        {
            Assert.IsTrue(Chain.Of(1, 2, 3) == Chain.Of(1, 2, 3));
            Assert.IsTrue(default(Chain<int>) == Chain.Of<int>());
        }

        [TestMethod]
        public void ChainComparison()
        {
            Assert.IsTrue(Chain.Compare(Chain.Of(1, 2, 3), Chain.Of(1, 2, 3)) == 0);
            Assert.IsTrue(Chain.Compare(Chain.Of<int>(), Chain.Of<int>()) == 0);
            Assert.IsTrue(Chain.Compare(Chain.Of(1, 2), Chain.Of(1, 2, 3)) < 0);
            Assert.IsTrue(Chain.Compare(Chain.Of(1, 2), Chain.Of<int>()) > 0);
            Assert.IsTrue(Chain.Compare(Chain.Of(7, 3, 5), Chain.Of(7, 4, 3)) < 0);
            Assert.IsTrue(Chain.Compare(default(Chain<String>), Chain.Of("hi")) < 0);
        }
    }
}
