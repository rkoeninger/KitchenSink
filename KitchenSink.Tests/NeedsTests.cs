using NUnit.Framework;

namespace KitchenSink.Tests
{
    [TestFixture]
    public class NeedsTests
    {
        [Test]
        public void LookupAllExplicit()
        {
            var x = new WS();
            var y = new DbW();
            var z = new DbR();
            var w = new UI();

            var needs = new Needs();
            needs.Add<IWebService>(x);
            needs.Add<IDatabaseCommand>(y);
            needs.Add<IDatabaseQuery>(z);
            needs.Add<IUserInterface>(w);

            Assert.AreEqual(z, needs.Get<IDatabaseQuery>());
            Assert.AreEqual(y, needs.Get<IDatabaseCommand>());
            Assert.AreEqual(w, needs.Get<IUserInterface>());
            Assert.AreEqual(x, needs.Get<IWebService>());
        }

        class DbW : IDatabaseCommand { }
        class DbR : IDatabaseQuery { }
        class UI : IUserInterface { }
        class WS : IWebService { }
        public interface IDatabaseQuery { }
        public interface IDatabaseCommand { }
        public interface IUserInterface { }
        public interface IWebService { }

        [Test]
        public void DefaultImplLookup()
        {
            var needs = new Needs();
            needs.Refer(typeof(NeedsTests));
            var thing = needs.Get<IAnInterface>();
            Assert.IsInstanceOf<AnImplementation>(thing);
        }
        
        public interface IAnInterface { }

        public class AnImplementation : IAnInterface { }

        [Test]
        public void DefaultImplOfLookup()
        {
            var needs = new Needs();
            needs.Refer(typeof(NeedsTests));
            var thing = needs.Get<ISomeInterface>();
            Assert.IsInstanceOf<SomeImplementation>(thing);
        }

        public interface ISomeInterface { }
        
        public class SomeImplementation : ISomeInterface { }

        [Test]
        public void MultipleDefaultImplsLookup()
        {
            var needs = new Needs();
            Expect.Error(() => needs.Get<IMyInterface>());
        }

        public interface IMyInterface { }
        
        public class MyImpl1 : IMyInterface { }
        
        public class MyImpl2 : IMyInterface { }

        [Test]
        public void NoImplAvailable()
        {
            var needs = new Needs();
            Expect.Error(() => needs.Get<IOtherInterface>());
        }

        public interface IOtherInterface { }
    }

}
