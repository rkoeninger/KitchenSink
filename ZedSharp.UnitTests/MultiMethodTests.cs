using System;
using NUnit.Framework;

namespace ZedSharp.UnitTests
{
    [TestFixture]
    public class MultiMethodTests
    {
        [Test]
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
