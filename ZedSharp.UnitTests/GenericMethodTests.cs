using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class GenericMethodTests
    {
        [TestMethod]
        public void PredicateDispatchTest()
        {
            var method = new GenericMethod<int, int>()
                .AddLast(Z.Neg, Z.Negate)
                .AddLast(_ => true, Z.Id)
                .AsFunc();

            Assert.AreEqual(5, method(-5));
            Assert.AreEqual(2, method(2));
            Assert.AreEqual(0, method(0));
        }
    }
}
