using System;
using KitchenSink.DI;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    [TestFixture]
    public class DependencyInjection
    {
        public interface InterfaceX
        {
        }

        public interface InterfaceY
        {
        }

        public class ParentClass
        {
            public class ImplementationX : InterfaceX
            {
            }

            public class ImplementationY : InterfaceY
            {
            }
        }

        public interface ISomeInterface { }

        public class SomeImplementation : ISomeInterface { }

        [Test]
        public void ReferringToNestedClass()
        {
            var needs = new Needs();
            needs.Refer(typeof(ParentClass));
            Assert.IsInstanceOf<ParentClass.ImplementationX>(needs.Get<InterfaceX>());
            Assert.IsInstanceOf<ParentClass.ImplementationY>(needs.Get<InterfaceY>());
        }

        [Test]
        public void ReferringToAssembly()
        {
            var needs = new Needs();
            needs.Refer(typeof(DependencyInjection).Assembly);
            Assert.IsInstanceOf<ParentClass.ImplementationX>(needs.Get<InterfaceX>());
            Assert.IsInstanceOf<ParentClass.ImplementationY>(needs.Get<InterfaceY>());
        }

        [Test]
        public void AddingInstance()
        {
            var needs = new Needs();
            needs.Add<ISomeInterface>(new SomeImplementation());
            Assert.IsInstanceOf<SomeImplementation>(needs.Get<ISomeInterface>());
        }

        [Test]
        public void AddingImplementingType()
        {
            var needs = new Needs();
            needs.Add<ISomeInterface>(typeof(SomeImplementation));
            Assert.IsInstanceOf<SomeImplementation>(needs.Get<ISomeInterface>());
        }

        [Test]
        public void DeferringToAnotherNeeds()
        {
            var needs0 = new Needs();
            needs0.Add<ISomeInterface>(new SomeImplementation());
            var needs1 = new Needs();
            needs1.Defer(needs0);
            Assert.IsInstanceOf<SomeImplementation>(needs1.Get<ISomeInterface>());
        }

        [Test]
        public void DeferringToArbitraryBackup()
        {
            var needs = new Needs();
            needs.Defer(_ => new SomeImplementation());
            Assert.IsInstanceOf<SomeImplementation>(needs.Get<ISomeInterface>());
        }

        [Test]
        public void ImplementationNotAvailable()
        {
            var needs = new Needs();
            Expect.Error<NotImplementedException>(() => needs.Get<ISomeInterface>());
        }
    }
}
