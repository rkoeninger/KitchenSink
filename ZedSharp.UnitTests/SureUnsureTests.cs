using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZedSharp;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class SureUnsureTests
    {
        public void AssertIsSome<A>(Unsure<A> unsure)
        {
            Assert.IsTrue(unsure.HasValue);
            Assert.IsFalse(unsure.HasError);
        }

        public void AssertIsNone<A>(Unsure<A> unsure)
        {
            Assert.IsFalse(unsure.HasValue);
            Assert.IsFalse(unsure.HasError);
        }

        public void AssertIsError<A>(Unsure<A> unsure)
        {
            Assert.IsFalse(unsure.HasValue);
            Assert.IsTrue(unsure.HasError);
        }

        [TestMethod]
        public void Wrappers()
        {
            String ns = null;
            String s = "";
            AssertIsNone(Unsure.Of(ns));
            AssertIsSome(Unsure.Of(s));
            AssertIsNone(Unsure.None<String>());
            AssertIsNone(Unsure.Error<String>(null));
            AssertIsError(Unsure.Error<String>(new Exception()));
        }

        [TestMethod]
        public void Parsing()
        {
            AssertIsSome("123".ToInt());
            AssertIsNone("!@#".ToInt());
            AssertIsNone("123.123".ToInt());
            AssertIsSome("123".ToDouble());
            AssertIsNone("!@#".ToDouble());
            AssertIsSome("123.123".ToDouble());
        }

        [TestMethod]
        public void EnumerableExtensions()
        {
            AssertIsSome(new [] {0}.UnsureFirst());
            AssertIsError(new int[0].UnsureFirst());
            AssertIsSome(new [] {0}.UnsureLast());
            AssertIsError(new int[0].UnsureLast());
            AssertIsSome(new [] {0}.UnsureSingle());
            AssertIsError(new int[0].UnsureSingle());
            AssertIsSome(new [] {0,0,0,0}.UnsureElementAt(2));
            AssertIsError(new [] {0,0,0,0}.UnsureElementAt(5));
            AssertIsError(new [] {0,0,0,0}.UnsureElementAt(-1));
            Assert.AreEqual(5, new [] {"#", "3", "2", "1", "e", "3", "r", "3"}.Select(x => x.ToInt()).WhereSure().Count());
            AssertIsNone(new [] {"#", "3", "2", "1", "e", "3", "r", "3"}.Select(x => x.ToInt()).Sequence());
            AssertIsSome(new [] {"9", "3", "2", "1", "6", "3", "5", "3"}.Select(x => x.ToInt()).Sequence());
        }
    }
}
