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
            var multi = MultiMethod.For(Z.Even).Return<String>()
                .Add(true, _ => "Even")
                .Add(false, _ => "Odd")
                .AsFunc();

            Assert.AreEqual("Even", multi(14));
            Assert.AreEqual("Odd", multi(-3));
        }
    }
}
