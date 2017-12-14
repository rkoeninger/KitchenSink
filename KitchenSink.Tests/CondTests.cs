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
        using static Operators;

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

    [TestFixture]
    public class CaseTests
    {
        [Test]
        public void DefaultMakesElseUnnecessary()
        {
            Assert.AreEqual("bye",
                Case.Of(0)
                    .Default("bye")
                    .When(1)
                    .Then("hi")
                    .When(2)
                    .Then("hello")
                    .When(3)
                    .Then("hey")
                    .End());
        }

        [Test]
        public void ClausesCanCastSubtypes()
        {
            Assert.AreEqual(1,
                Case.Of<IInterfaceX>(new ImplA { ValA = 1 })
                    .When<ImplA>()
                    .Then(x => x.ValA)
                    .When<ImplB>(x => x.ValB != null)
                    .Then(x => x.ValB.Length)
                    .Else(0));
        }

        public interface IInterfaceX
        {
        }

        public class ImplA : IInterfaceX
        {
            public int ValA { get; set; }
        }

        public class ImplB : IInterfaceX
        {
            public string ValB { get; set; }
        }
    }
}
