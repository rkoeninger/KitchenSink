using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class StringTests
    {
        [TestMethod]
        public void StringSeqTest()
        {
            var s = "comma, separated, string with multiple, comma, separated, parts";
            var seq = s.SplitSeq(",");
            Assert.AreEqual(6, seq.Count());
        }
    }
}
