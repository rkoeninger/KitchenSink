using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class MaybeTests
    {
        [TestMethod]
        public void MaybeWrappers()
        {
            String ns = null;
            String s = "";
            AssertIsNone(Maybe.Of(ns));
            AssertIsSome(Maybe.Of(s));
            AssertIsNone(Maybe<String>.None);
        }

        [TestMethod]
        public void MaybeParsing()
        {
            AssertIsSome("123".ToInt());
            AssertIsNone("!@#".ToInt());
            AssertIsNone("123.123".ToInt());
            AssertIsSome("123".ToDouble());
            AssertIsNone("!@#".ToDouble());
            AssertIsSome("123.123".ToDouble());
        }

        [TestMethod]
        public void MaybeJoining()
        {
            Assert.AreEqual(Maybe.Of(3), Maybe.Of(1).Join(Maybe.Of(2), Z.Add));
            Assert.AreEqual(Maybe<int>.None, Maybe<int>.None.Join(Maybe.Of(2), Z.Add));
            Assert.AreEqual(Maybe<int>.None, Maybe.Of(1).Join(Maybe<int>.None, Z.Add));
            Assert.AreEqual(Maybe<int>.None, Maybe<int>.None.Join(Maybe<int>.None, Z.Add));
        }

        [TestMethod]
        public void MaybeCasting()
        {
            Assert.IsTrue(Maybe.Of("").Cast<string>().HasValue); // same-type
            Assert.IsTrue(Maybe.Of("").Cast<object>().HasValue); // up-casting
            Assert.IsFalse(Maybe.Of(new object()).Cast<string>().HasValue); // down-casting
            Assert.IsFalse(Maybe.Of("").Cast<int>().HasValue); // casting to unrelated type
        }

        [TestMethod]
        public void MaybeEnumerableExtensions()
        {
            AssertIsSome(new [] {0}.FirstMaybe());
            AssertIsNone(new int[0].FirstMaybe());
            AssertIsSome(new [] {0}.LastMaybe());
            AssertIsNone(new int[0].LastMaybe());
            AssertIsSome(new [] {0}.SingleMaybe());
            AssertIsNone(new int[0].SingleMaybe());
            AssertIsSome(new [] {0,0,0,0}.ElementAtMaybe(2));
            AssertIsNone(new[] { 0, 0, 0, 0 }.ElementAtMaybe(5));
            AssertIsNone(new[] { 0, 0, 0, 0 }.ElementAtMaybe(-1));
            Assert.AreEqual(5, new [] {"#", "3", "2", "1", "e", "3", "r", "3"}.Select(Maybe.ToInt).Flatten().Count());
            AssertIsNone(new [] {"#", "3", "2", "1", "e", "3", "r", "3"}.Select(Maybe.ToInt).Sequence());
            AssertIsSome(new [] {"9", "3", "2", "1", "6", "3", "5", "3"}.Select(Maybe.ToInt).Sequence());
        }

        [TestMethod]
        public void ValidationTests()
        {
            var v = Validation.Of("abcdefg")
                .Is(x => x.StartsWith("abc"))
                .Is(x => x.EndsWith("abc"))
                .Is(x => { if (x.Length < 10) throw new Exception("asdfasdfa"); });
            Assert.AreEqual(2, v.Errors.Count());
        }

        public void AssertIsSome<A>(Maybe<A> maybe)
        {
            Assert.IsTrue(maybe.HasValue);
        }

        public void AssertIsNone<A>(Maybe<A> maybe)
        {
            Assert.IsFalse(maybe.HasValue);
        }
    }
}
