using NUnit.Framework;

namespace KitchenSink.Tests
{
    public class DependencyInjection
    {
        public interface IInterfaceX { }
        public interface IInterfaceY { }

        public class ParentClass
        {
            public class ImplementationX : IInterfaceX { }
            public class ImplementationY : IInterfaceY { }
        }

        public interface ISomeInterface { }
        public class SomeImplementation : ISomeInterface { }
        
        [SingleUse]
        public class SingleUseImplementation : IWhateverInterface { }
        public interface IWhateverInterface { }
        public interface IAnotherInterface { }
        public class DependentImplementation : IAnotherInterface
        {
            public DependentImplementation(IWhateverInterface whatever) { }
        }

        public interface IBlahInterface { }
        public class BlahImplementation : IBlahInterface
        {
            public BlahImplementation() { }
            public BlahImplementation(ISomeInterface some) { }
        }

        [Test]
        public void ReferringToNestedClass()
        {
            var needs = new Needs().ReferChildren(typeof(ParentClass));
            Assert.IsInstanceOf<ParentClass.ImplementationX>(needs.Get<IInterfaceX>());
            Assert.IsInstanceOf<ParentClass.ImplementationY>(needs.Get<IInterfaceY>());
        }

        [Test]
        public void ReferringToAssembly()
        {
            var needs = new Needs().Refer(typeof(DependencyInjection).Assembly);
            Assert.IsInstanceOf<ParentClass.ImplementationX>(needs.Get<IInterfaceX>());
            Assert.IsInstanceOf<ParentClass.ImplementationY>(needs.Get<IInterfaceY>());
        }

        [Test]
        public void AddingInstance()
        {
            var needs = new Needs().Add<ISomeInterface>(new SomeImplementation());
            Assert.IsInstanceOf<SomeImplementation>(needs.Get<ISomeInterface>());
        }

        [Test]
        public void AddingImplementingTypeAsTypeObject()
        {
            var needs = new Needs().Add<ISomeInterface>(typeof(SomeImplementation));
            Assert.IsInstanceOf<SomeImplementation>(needs.Get<ISomeInterface>());
        }

        [Test]
        public void AddingImplementingTypeAsTypeParameter()
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
            var needs = new Needs().ReferChildren(typeof(DependencyInjection));
            Expect.Error<ImplementationReliabilityException>(() => needs.Get<IAnotherInterface>());
        }

        [Test]
        public void ImplementationWithMultipleConstructors()
        {
            var needs = new Needs().ReferChildren(typeof(DependencyInjection));
            Expect.Error<MultipleConstructorsException>(() => needs.Get<IBlahInterface>());
        }

        public class TypeSpecialized
        {
            public sealed class PlumVariant : NewType<ISomeInterface>
            {
                public PlumVariant(ISomeInterface value) : base(value)
                {
                }
            }

            public sealed class PearVariant : NewType<ISomeInterface>
            {
                public PearVariant(ISomeInterface value) : base(value)
                {
                }
            }

            public class VariantDependent
            {
                // ReSharper disable UnusedParameter.Local
                public VariantDependent(PlumVariant plum, PearVariant pear)
                {
                }
                // ReSharper restore UnusedParameter.Local
            }

            [Test]
            public void ImplementationsCanBeDeterminedByNewType()
            {
                var needs = new Needs();
                needs.Add(new PearVariant(new SomeImplementation()));
                needs.Add(new PlumVariant(new SomeImplementation()));
                Assert.AreNotEqual(needs.Get<PearVariant>().Value, needs.Get<PlumVariant>().Value);
            }
        }

        public class Decoration
        {
            public interface IFacility
            {
                int Facilitate();
            }

            public class BasicFacility : IFacility
            {
                public int Facilitate() => 1;
            }

            public class AdvancedFacility : IFacility
            {
                private readonly IFacility inner;

                public AdvancedFacility(IFacility inner)
                {
                    this.inner = inner;
                }

                public int Facilitate() => inner.Facilitate() + 1;
            }

            [Test]
            public void SimpleMultiUseDecoration()
            {
                var needs = new Needs();
                needs.Add<IFacility, BasicFacility>();
                Assert.AreEqual(1, needs.Get<IFacility>().Facilitate());
                needs.Decorate<IFacility, AdvancedFacility>();
                Assert.AreEqual(2, needs.Get<IFacility>().Facilitate());

                // Decorator can be re-applied/re-wrapped multiple times
                needs.Decorate<IFacility, AdvancedFacility>();
                Assert.AreEqual(3, needs.Get<IFacility>().Facilitate());
            }
        }
    }
}
