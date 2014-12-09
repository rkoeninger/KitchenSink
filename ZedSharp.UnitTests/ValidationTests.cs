using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class ValidationTests
    {
        [TestMethod]
        public void ValidationPropertyChain()
        {
            var a = new Person("a", "b", null);
            var b = new Person("a", "b", new Address("123", null));
            var c = new Person("a", "b", new Address("123", "qwerty"));
            Assert.IsFalse(Verify.That(() => a.Address.City));
            Assert.IsFalse(Verify.That(() => b.Address.City));
            Assert.IsTrue(Verify.That(() => c.Address.City));
            Assert.IsFalse(Verify.That(() => c.Address.City == "springfield"));
            Assert.IsTrue(Verify.That(() => c.Address.City == c.Address.City));
            Assert.IsFalse(Verify.That(() => b.Address.City == b.Address.City));
            Assert.IsFalse(Verify.That(() => a.Address.City == a.Address.City));
            Assert.IsTrue(Verify.That(() => b.Address.City.NoVerify()));
        }
    }
}
