using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class ValidationTests
    {
        [TestMethod]
        public void CheckTests()
        {
            var v = Validation.Of("abcdefg")
                .Check(x => x.StartsWith("abc"))
                .Check(x => x.EndsWith("abc"))
                .Check(x => { if (x.Length < 10) throw new Exception("asdfasdfa"); });
            Assert.AreEqual(2, v.Errors.Count());
        }
    }
}
