using KitchenSink.Injection;
using KitchenSink.Testing;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    [TestFixture]
    public class DependencyInjection
    {
        public interface InterfaceX { }
        public interface InterfaceY { }

        public class ParentClass
        {
            public class ImplementationX : InterfaceX { }
            public class ImplementationY : InterfaceY { }
        }

        public interface ISomeInterface { }
        public class SomeImplementation : ISomeInterface { }
        
        [SingleUse]
        public class SingleUseImplementation : IWhateverInterface { }
        public interface IWhateverInterface { }
        public interface IAnotherInterface { }
        public class DependentImplementation : IAnotherInterface
        {
            // ReSharper disable once UnusedParameter.Local
            public DependentImplementation(IWhateverInterface whatever) { }
        }

        public interface IBlahInterface { }
        public class BlahImplementation : IBlahInterface
        {
            public BlahImplementation() { }
            // ReSharper disable once UnusedParameter.Local
            public BlahImplementation(ISomeInterface some) { }
        }

        [Test]
        public void ReferringToNestedClass()
        {
            var needs = new Needs().Refer(typeof(ParentClass));
            Assert.IsInstanceOf<ParentClass.ImplementationX>(needs.Get<InterfaceX>());
            Assert.IsInstanceOf<ParentClass.ImplementationY>(needs.Get<InterfaceY>());
        }

        [Test]
        public void ReferringToAssembly()
        {
            var needs = new Needs().Refer(typeof(DependencyInjection).Assembly);
            Assert.IsInstanceOf<ParentClass.ImplementationX>(needs.Get<InterfaceX>());
            Assert.IsInstanceOf<ParentClass.ImplementationY>(needs.Get<InterfaceY>());
        }

        [Test]
        public void AddingInstance()
        {
            var needs = new Needs().Add<ISomeInterface>(new SomeImplementation());
            Assert.IsInstanceOf<SomeImplementation>(needs.Get<ISomeInterface>());
        }

        [Test]
        public void AddingImplementingType()
        {
            var needs = new Needs().Add<ISomeInterface>(typeof(SomeImplementation));
            Assert.IsInstanceOf<SomeImplementation>(needs.Get<ISomeInterface>());
        }

        [Test]
        public void DeferringToAnotherNeeds()
        {
            var needs0 = new Needs().Add<ISomeInterface>(new SomeImplementation());
            var needs1 = new Needs().Defer(needs0);
            Assert.IsInstanceOf<SomeImplementation>(needs1.Get<ISomeInterface>());
        }

        [Test]
        public void DeferringToArbitraryBackup()
        {
            var needs = new Needs().Defer(_ => new SomeImplementation());
            Assert.IsInstanceOf<SomeImplementation>(needs.Get<ISomeInterface>());
        }

        [Test]
        public void ImplementationNotAvailable()
        {
            var needs = new Needs();
            Expect.Error<ImplementationUnresolvedException>(() => needs.Get<ISomeInterface>());
        }

        [Test]
        public void ImplementationNotAvailableInDeferredNeeds()
        {
            var needs0 = new Needs();
            var needs1 = new Needs().Defer(needs0);
            Expect.Error<ImplementationUnresolvedException>(() => needs1.Get<ISomeInterface>());
        }

        [Test]
        public void MultiUseCantDependOnSingleUse()
        {
            var needs = new Needs().Refer(typeof(DependencyInjection));
            Expect.Error<ImplementationReliabilityException>(() => needs.Get<IAnotherInterface>());
        }

        [Test]
        public void ImplementationWithMultipleConstructors()
        {
            var needs = new Needs().Refer(typeof(DependencyInjection));
            Expect.Error<MultipleConstructorsException>(() => needs.Get<IBlahInterface>());
        }
    }
}
