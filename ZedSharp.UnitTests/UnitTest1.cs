using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZedSharp;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            String ns = null;
            String s = "";
            Assert.IsFalse(Unsure.Of(ns).HasValue);
            Assert.IsTrue(Unsure.Of(s).HasValue);
            Assert.IsFalse(Unsure.None<String>().HasValue);
            Assert.IsFalse(Unsure.Error<String>(new Exception()).HasValue);
        }
    }
}
