using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZedSharp.Test;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class MatchTests
    {
        [TestMethod]
        public void ValueMatching()
        {
            Assert.IsFalse(Match.On("").Return<int>().End().HasValue);
            Assert.IsTrue(Match.On("").Return<int>().Case("").Then(0).End().HasValue);
            Assert.IsTrue(Match.On("").Case("").Then(0).End().HasValue);
            Assert.IsFalse(Match.On("").Case("x").Then(0).End().HasValue);
            Maybe<int> ui = Match.On("").Case("").Then(0);
            Assert.IsTrue(ui.HasValue);

            var m1 = Match.On("abc").Case("def").Then(0).Case("ghi").Then(1);
            var res1 = m1.Case("abc").Then(2).Case("jkl").Then(3).End();
            Assert.AreEqual(Maybe.Of(2), res1);

            // Result type is inferred from first Then() to be an Int32 so false, a Boolean, in the second Then() is invalid
            Expect.CompileFail(Common.Wrap(@"Match.On("""").Case(""x"").Then(0).Case(""y"").Then(false).End()"), Common.ZedDll);

            // Can't call Case or Then twice in a row
            Expect.CompileFail(Common.Wrap(@"Match.On("""").Case(""x"").Case(""z"").Then(0).End()"), Common.ZedDll);
            Expect.CompileFail(Common.Wrap(@"Match.On("""").Case(""x"").Then(1).Then(0).End()"), Common.ZedDll);

            List<String> l;
            l = new List<String>();
            var res2 = Match.On(4)
                .Case(Track<int, bool>(l, "case1", 1.Eq())).Then(Track<int, string>(l, "then1", _ => "one"))
                .Case(Track<int, bool>(l, "case2", 2.Eq())).Then(Track<int, string>(l, "then2", _ => "two"))
                .Case(Track<int, bool>(l, "case3", 3.Eq())).Then(Track<int, string>(l, "then3", _ => "three"))
                .Case(Track<int, bool>(l, "case4", 4.Eq())).Then(Track<int, string>(l, "then4", _ => "four"))
                .Case(Track<int, bool>(l, "case5", 5.Eq())).Then(Track<int, string>(l, "then5", _ => "five"))
                .End();
            Assert.AreEqual(Maybe.Of("four"), res2);
            Assert.IsTrue(l.SequenceEqual(Seq.Of("case1", "case2", "case3", "case4", "then4")));

            Assert.AreEqual(31, Match.On("abc").Case("def").Then(23).Else(31));

            Assert.AreEqual("whatever", Match.On(3).Default("whatever").End());
            Assert.AreEqual("whatever", Match.On(3).Default("whatever").Case(5).Then("asdf").Case(2).Then("awert").End());
            Assert.AreEqual("fgjh", Match.On(3).Default("whatever").Case(5).Then("asdf").Case(3).Then("fgjh").Case(2).Then("awert").End());

            l = new List<String>();
            var res3 = Match.On(3)
                .Default(Track<int, string>(l, "default", _ => "whatever"))
                .Case(Track<int, bool>(l, "case1", 5.Eq())).Then(Track<int, string>(l, "then1", _ => "asdf"))
                .Case(Track<int, bool>(l, "case2", 3.Eq())).Then(Track<int, string>(l, "then2", _ => "fgjh"))
                .Case(Track<int, bool>(l, "case3", 2.Eq())).Then(Track<int, string>(l, "then3", _ => "awert"))
                .End();
            Assert.AreEqual("fgjh", res3);
            Assert.IsTrue(l.SequenceEqual(Seq.Of("case1", "case2", "then2")));

            l = new List<String>();
            var res4 = Match.On(3)
                .Default(Track<int, string>(l, "default", _ => "whatever"))
                .Case(Track<int, bool>(l, "case1", 5.Eq())).Then(Track<int, string>(l, "then1", _ => "asdf"))
                .Case(Track<int, bool>(l, "case2", 1.Eq())).Then(Track<int, string>(l, "then2", _ => "fgjh"))
                .Case(Track<int, bool>(l, "case3", 7.Eq())).Then(Track<int, string>(l, "then3", _ => "sdfgg"))
                .Case(Track<int, bool>(l, "case4", 2.Eq())).Then(Track<int, string>(l, "then4", _ => "awert"))
                .End();
            Assert.AreEqual("whatever", res4);
            Assert.IsTrue(l.SequenceEqual(Seq.Of("case1", "case2", "case3", "case4", "default")));

            var f = Match<int>.Case(1).Then("qwerty").Case(4).Then("lkjhg").Case(7).Then("zxcvb").Else("poiuy");
            Assert.AreEqual("qwerty", f(1));
            Assert.AreEqual("zxcvb", f(7));
            Assert.AreEqual("poiuy", f(3));

            var g = Match<int>.Default("poiuy").Case(1).Then("qwerty").Case(4).Then("lkjhg").Case(7).Then("zxcvb").End();
            Assert.AreEqual("qwerty", g(1));
            Assert.AreEqual("zxcvb", g(7));
            Assert.AreEqual("poiuy", g(3));

        }

        [TestMethod]
        public void MatchEvalOrder()
        {
            var res = Match.On(3).Case(1).Then("a").Case(3).Then("b").Case(3).Then("c").Else("d");
            Assert.AreEqual("b", res);
        }

        [TestMethod]
        public void MatchFuncEvalOrder()
        {
            var res = Match.From<int>().Case(1).Then("a").Case(3).Then("b").Case(3).Then("c").Else("d");
            Assert.AreEqual("b", res(3));
        }

        [TestMethod]
        public void MatchOnOff()
        {
            var res12 = Match.On(5)
                .Case(1).Then("a")
                .Case(2).Then("b")
                .Case(3).Then("c")
                .Swap(12)
                .Case(4).Then("d")
                .Case(5).Then("e")
                .Swap(7)
                .Case(6).Then("f")
                .Case(7).Then("g")
                .Case(8).Then("h")
                .Swap(9)
                .Case(9).Then("i")
                .Case(10).Then("j")
                .Else("qwerty");
            Assert.AreEqual("g", res12);
        }

        private Func<A, B> Track<A, B>(List<String> l, String msg, Func<A, B> f)
        {
            return x => {
                l.Add(msg);
                return f(x);
            };
        }
    }
}
