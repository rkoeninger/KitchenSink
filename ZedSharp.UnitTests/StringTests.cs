using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
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
            Assert.IsTrue(seq.TrimAll().SequenceEqual(Z.Seq("comma", "separated", "string with multiple", "comma", "separated", "parts")));
        }

        [TestMethod]
        public void StringRegexSeq()
        {
            var usPhoneRegex = new Regex(@"(\x28?\d{3}\x29?[\x20\x2D\x2E])?(\d{3})[\x20\x2D\x2E](\d{4})");
            var s = "some text that (123) 555-1234 contains some U.S. phone 432.6545 numbers of varying 654 234 1233 formats";
            var seq = s.SplitSeq(usPhoneRegex);
            Assert.AreEqual(3, seq.Count());
        }
    }
}
