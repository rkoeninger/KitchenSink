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
            Unsure<int> ui = Match.On("").Case("").Then(0);
            Assert.IsTrue(ui.HasValue);

            var m1 = Match.On("abc").Case("def").Then(0).Case("ghi").Then(1);
            Assert.IsFalse(m1.IsComplete);
            var res1 = m1.Case("abc").Then(2).Case("jkl").Then(3).End();
            Assert.IsFalse(m1.IsComplete);
            Assert.AreEqual(Unsure.Of(2), res1);

            // Result type is inferred from first Then() to be an Int32 so false, a Boolean, in the second Then() is invalid
            Expect.CompileFail(Common.Wrap(@"Match.On("""").Case(""x"").Then(0).Case(""y"").Then(false).End()"), Common.ZedDll);

            // Can't call Case or Then twice in a row
            Expect.CompileFail(Common.Wrap(@"Match.On("""").Case(""x"").Case(""z"").Then(0).End()"), Common.ZedDll);
            Expect.CompileFail(Common.Wrap(@"Match.On("""").Case(""x"").Then(1).Then(0).End()"), Common.ZedDll);
        }
    }
}
