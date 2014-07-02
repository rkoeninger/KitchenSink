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

            // Result type is inferred from first Then() to be an Int32 so false, a Boolean, in the second Then() is invalid
            Expect.CompileFail(Common.Wrap(@"Match.On("""").Case(""x"").Then(0).Case(""y"").Then(false).End()"), Common.ZedDll);
        }
    }
}
