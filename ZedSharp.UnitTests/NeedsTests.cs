using System;
using NUnit.Framework;

namespace ZedSharp.UnitTests
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
            needs.Set<IWebService>(x);
            needs.Set<IDatabaseCommand>(y);
            needs.Set<IDatabaseQuery>(z);
            needs.Set<IUserInterface>(w);

            Expect.Some(z, needs.Get<IDatabaseQuery>());
            Expect.Some(y, needs.Get<IDatabaseCommand>());
            Expect.Some(w, needs.Get<IUserInterface>());
            Expect.Some(x, needs.Get<IWebService>());
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
            var thing = needs.Get<IAnInterface>().OrElseThrow(() => new Exception("No impl found"));
            Assert.IsInstanceOf<AnImplementation>(thing);
        }

        [DefaultImplementation(typeof(AnImplementation))]
        public interface IAnInterface { }

        public class AnImplementation : IAnInterface { }

        [Test]
        public void DefaultImplOfLookup()
        {
            var needs = new Needs();
            var thing = needs.Get<ISomeInterface>().OrElseThrow(() => new Exception("No impl found"));
            Assert.IsInstanceOf<SomeImplementation>(thing);
        }

        public interface ISomeInterface { }

        [DefaultImplementationOf(typeof(ISomeInterface))]
        public class SomeImplementation : ISomeInterface { }

        [Test]
        public void MultipleDefaultImplsLookup()
        {
            var needs = new Needs();
            Expect.Error(() => needs.Get<IMyInterface>());
        }

        public interface IMyInterface { }

        [DefaultImplementationOf(typeof(IMyInterface))]
        public class MyImpl1 : IMyInterface { }

        [DefaultImplementationOf(typeof(IMyInterface))]
        public class MyImpl2 : IMyInterface { }

        [Test]
        public void NoImplAvailable()
        {
            var needs = new Needs();
            Expect.None(needs.Get<IOtherInterface>());
        }

        public interface IOtherInterface { }
    }

}
