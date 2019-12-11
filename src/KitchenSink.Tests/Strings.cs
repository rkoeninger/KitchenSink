using System.Linq;
using System.Text.RegularExpressions;
using static KitchenSink.Operators;
using KitchenSink.Extensions;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    public class Strings
    {
        [Test]
        public void StringSeqTest()
        {
            const string s = "comma, separated, string with multiple, comma, separated, parts";
            var splits = s.Split(",").ToList();
            Assert.AreEqual(6, splits.Count);
        }

        [Test]
        public void StringRegexSeq()
        {
            var usPhoneRegex = new Regex(@"(\x28?\d{3}\x29?[\x20\x2D\x2E])?(\d{3})[\x20\x2D\x2E](\d{4})");
            const string s = "some text that (123) 555-1234 contains some U.S. phone 432.6545 numbers of varying 654 234 1233 formats";
            var splits = s.Split(usPhoneRegex).ToList();
            Assert.AreEqual(3, splits.Count);
            Assert.IsTrue(splits.SequenceEqual(SeqOf("(123) 555-1234", "432.6545", "654 234 1233")));
        }

        [Test]
        public void CollapseWhiteSpace()
        {
            Assert.AreEqual("asd fdjkv sdfv fsv as4 '", " asd   fdjkv sdfv \nfsv \r\ras4 '  ".CollapseSpace());
        }

        [Test]
        public void NamingConventions()
        {
            Assert.AreEqual("", ((string)null).ToLispCase());
            Assert.AreEqual("", "".ToLispCase());
            Assert.AreEqual("", "     ".ToLispCase());
            Assert.AreEqual("abcdefghi", "Abcdefghi".ToLispCase());
            Assert.AreEqual("abcdefghi", "abcdefghi".ToLispCase());
            Assert.AreEqual("abc-def-ghi", "AbcDefGhi".ToLispCase());
            Assert.AreEqual("abc-def-ghi", "abcDefGhi".ToLispCase());
            Assert.AreEqual("abc-def-ghi", "AbcDEFGhi".ToLispCase());
            Assert.AreEqual("abc-def-ghi", "abcDEFGhi".ToLispCase());
            Assert.AreEqual("abc-def-ghi", "abcDefGHI".ToLispCase());
            Assert.AreEqual("abc-def-ghi", "ABCDefGHI".ToLispCase());
            Assert.AreEqual("abc2-ghi", "abc2Ghi".ToLispCase());
            Assert.AreEqual("abc2-ghi", "Abc2Ghi".ToLispCase());
            Assert.AreEqual("abc2-ghi", "ABC2Ghi".ToLispCase());
            Assert.AreEqual("abc2-ghi", "abc2GHI".ToLispCase());

            Assert.AreEqual("", ((string)null).ToSnakeCase());
            Assert.AreEqual("", "".ToSnakeCase());
            Assert.AreEqual("", "     ".ToSnakeCase());
            Assert.AreEqual("abcdefghi", "Abcdefghi".ToSnakeCase());
            Assert.AreEqual("abcdefghi", "abcdefghi".ToSnakeCase());
            Assert.AreEqual("abc_def_ghi", "AbcDefGhi".ToSnakeCase());
            Assert.AreEqual("abc_def_ghi", "abcDefGhi".ToSnakeCase());
            Assert.AreEqual("abc_def_ghi", "AbcDEFGhi".ToSnakeCase());
            Assert.AreEqual("abc_def_ghi", "abcDEFGhi".ToSnakeCase());
            Assert.AreEqual("abc_def_ghi", "abcDefGHI".ToSnakeCase());
            Assert.AreEqual("abc_def_ghi", "ABCDefGHI".ToSnakeCase());
            Assert.AreEqual("abc2_ghi", "abc2Ghi".ToSnakeCase());
            Assert.AreEqual("abc2_ghi", "Abc2Ghi".ToSnakeCase());
            Assert.AreEqual("abc2_ghi", "ABC2Ghi".ToSnakeCase());
            Assert.AreEqual("abc2_ghi", "abc2GHI".ToSnakeCase());
        }
    }
}
