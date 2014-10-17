using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class MultiMethodTests
    {
        [TestMethod]
        public void SimpleDispatchTest()
        {
            var multi = MultiMethod.For((int x) => x % 2 == 0).Return<String>()
                .Add(true, _ => "Even")
                .Add(false, _ => "Odd");

            Assert.AreEqual("Even", multi.Apply(14));
            Assert.AreEqual("Odd", multi.Apply(-3));
        }
    }
}
