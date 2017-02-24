using KitchenSink.Control;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    [TestFixture]
    public class CondTests
    {
        [TestFixture]
        public class SpecifyingReturnType
        {
            [Test]
            public void InferResultTypeFromFirstConsequent()
            {
                Assert.AreEqual(0, Cond.If(() => true).Then(() => 0).Else(() => 1));
            }

            [Test]
            public void ExplicitResultOnFirstCondition()
            {
                Assert.AreEqual(0, Cond.If<int>(() => true).Then(() => 0).Else(() => 1));
            }

            [Test]
            public void ExplicitResultOnCondClassName()
            {
                Assert.AreEqual(0, Cond<int>.If(() => true).Then(() => 0).Else(() => 1));
            }
        }

        [Test]
        public void AbsorbtionAddsAllClauses()
        {
            Assert.AreEqual(
                3,
                Cond.If(false).Then(0)
                    .If(false).Then(1)
                    .Absorb(
                        Cond.If(false).Then(2)
                            .If(true).Then(3)
                            .If(false).Then(4))
                    .If(false).Then(5)
                    .Else(-1));
        }
    }

    namespace UsingStatic
    {
        using static Cond;

        [TestFixture]
        public class CondUsingStaticTests
        {
            [Test]
            public void StartingIfMethodCanBeCalledUnqualified()
            {
                If(true).Then().Else();
            }

            [Test]
            public void StartingIfMethodCanBeCalledUnqualifiedWithExplicitResultType()
            {
                If<int>(true).Then(1).Else(0);
            }
        }
    }
}
