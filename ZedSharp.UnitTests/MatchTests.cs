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

            var l = new List<String>();

            var res2 = Match.On(4)
                .Case(Track<int, bool>(l, "case1", x => x == 1)).Then(Track<int, string>(l, "then1", _ => "one"))
                .Case(Track<int, bool>(l, "case2", x => x == 2)).Then(Track<int, string>(l, "then2", _ => "two"))
                .Case(Track<int, bool>(l, "case3", x => x == 3)).Then(Track<int, string>(l, "then3", _ => "three"))
                .Case(Track<int, bool>(l, "case4", x => x == 4)).Then(Track<int, string>(l, "then4", _ => "four"))
                .Case(Track<int, bool>(l, "case5", x => x == 5)).Then(Track<int, string>(l, "then5", _ => "five"))
                .End();
            Assert.AreEqual(Maybe.Of("four"), res2);
            Assert.IsTrue(l.SequenceEqual(new [] {"case1", "case2", "case3", "case4", "then4"}));

            Assert.AreEqual(Maybe.Of(31), Match.On("abc").Case("def").Then(23).Else().Then(31));
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
