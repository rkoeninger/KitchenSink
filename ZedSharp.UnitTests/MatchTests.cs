using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class MatchTests
    {
        [TestMethod]
        public void MatchBasics()
        {
            Assert.IsFalse(Match.On("").Return<int>().End.HasValue);
            Assert.IsTrue(Match.On("").Return<int>().Case("", 0).End.HasValue);
            Assert.IsTrue(Match.On("").Case("", 0).End.HasValue);
            Assert.IsFalse(Match.On("").Case("x", 0).End.HasValue);
            Maybe<int> ui = Match.On("").Case("", 0).End;
            Assert.IsTrue(ui.HasValue);

            var m1 = Match.On("abc").Case("def", 0).Case("ghi", 1);
            var res1 = m1.Case("abc", 2).Case("jkl", 3).End;
            Assert.AreEqual(Maybe.Of(2), res1);

            Assert.AreEqual(31, Match.On("abc").Case("def", 23).Else(31));
        }

        [TestMethod]
        public void MatchTypeCorrectness()
        {
            // Result type is inferred from first Then() to be an Int32 so false, a Boolean, in the second Then() is invalid
            Expect.CompileFail(
                Common.Wrap(@"Match.On("""").Case(""x"", 0).Case(""y"", false).End()"),
                new [] {Common.ZedDll},
                new []
                {
                    "CS1502", // Case has some invalid arguments
                    "CS1503"  // Cannot convert from bool to Func<string, int> in second Case
                });
        }

        [TestMethod]
        public void MatchTrackingEvalOrder()
        {
            var l = new List<String>();
            var res2 = Match.On(4)
                .Case(Track(l, "case1", 1.Eq()), Track<int, string>(l, "then1", _ => "one"))
                .Case(Track(l, "case2", 2.Eq()), Track<int, string>(l, "then2", _ => "two"))
                .Case(Track(l, "case3", 3.Eq()), Track<int, string>(l, "then3", _ => "three"))
                .Case(Track(l, "case4", 4.Eq()), Track<int, string>(l, "then4", _ => "four"))
                .Case(Track(l, "case5", 5.Eq()), Track<int, string>(l, "then5", _ => "five"))
                .End;
            Assert.AreEqual(Maybe.Of("four"), res2);
            Assert.IsTrue(l.SequenceEqual(Seq.Of("case1", "case2", "case3", "case4", "then4")));
        }

        [TestMethod]
        public void MatchDefault()
        {
            Assert.AreEqual("whatever", Match.On(3).Default("whatever").End);
            Assert.AreEqual("whatever", Match.On(3).Default("whatever").Case(5, "asdf").Case(2, "awert").End);
            Assert.AreEqual("fgjh", Match.On(3).Default("whatever").Case(5, "asdf").Case(3, "fgjh").Case(2, "awert").End);
        }

        [TestMethod]
        public void MatchDefaultTrackingEvalOrder()
        {
            var l = new List<String>();
            var res3 = Match.On(3)
                .Default(Track<int, string>(l, "default", _ => "whatever"))
                .Case(Track(l, "case1", 5.Eq()), Track<int, string>(l, "then1", _ => "asdf"))
                .Case(Track(l, "case2", 3.Eq()), Track<int, string>(l, "then2", _ => "fgjh"))
                .Case(Track(l, "case3", 2.Eq()), Track<int, string>(l, "then3", _ => "awert"))
                .End;
            Assert.AreEqual("fgjh", res3);
            Assert.IsTrue(l.SequenceEqual(Seq.Of("case1", "case2", "then2")));

            l = new List<String>();
            var res4 = Match.On(3)
                .Default(Track<int, string>(l, "default", _ => "whatever"))
                .Case(Track(l, "case1", 5.Eq()), Track<int, string>(l, "then1", _ => "asdf"))
                .Case(Track(l, "case2", 1.Eq()), Track<int, string>(l, "then2", _ => "fgjh"))
                .Case(Track(l, "case3", 7.Eq()), Track<int, string>(l, "then3", _ => "sdfgg"))
                .Case(Track(l, "case4", 2.Eq()), Track<int, string>(l, "then4", _ => "awert"))
                .End;
            Assert.AreEqual("whatever", res4);
            Assert.IsTrue(l.SequenceEqual(Seq.Of("case1", "case2", "case3", "case4", "default")));
        }

        [TestMethod]
        public void MatchFunc()
        {
            var f = Match<int>.Case(1, "qwerty").Case(4, "lkjhg").Case(7, "zxcvb").Else("poiuy");
            Assert.AreEqual("qwerty", f(1));
            Assert.AreEqual("zxcvb", f(7));
            Assert.AreEqual("poiuy", f(3));

            var g = Match<int>.Default("poiuy").Case(1, "qwerty").Case(4, "lkjhg").Case(7, "zxcvb").End;
            Assert.AreEqual("qwerty", g(1));
            Assert.AreEqual("zxcvb", g(7));
            Assert.AreEqual("poiuy", g(3));
        }

        [TestMethod]
        public void MatchEvalOrder()
        {
            var res = Match.On(3).Case(1, "a").Case(3, "b").Case(3, "c").Else("d");
            Assert.AreEqual("b", res);
        }

        [TestMethod]
        public void MatchFuncEvalOrder()
        {
            var res = Match.From<int>().Case(1, "a").Case(3, "b").Case(3, "c").Else("d");
            Assert.AreEqual("b", res(3));
        }

        private static Func<A, B> Track<A, B>(List<String> l, String msg, Func<A, B> f)
        {
            return x => {
                l.Add(msg);
                return f(x);
            };
        }
    }
}
