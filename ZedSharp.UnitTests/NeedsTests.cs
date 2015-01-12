using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class NeedsTests
    {
        [TestMethod]
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

        [TestMethod]
        public void DefaultImplLookup()
        {
            var needs = new Needs();
            var thing = needs.Get<IAnInterface>().OrElseThrow(() => new Exception("No impl found"));
            Assert.IsInstanceOfType(thing, typeof(AnImplementation));
        }

        [DefaultImplementation(typeof(AnImplementation))]
        public interface IAnInterface { }

        public class AnImplementation : IAnInterface { }

        [TestMethod]
        public void DefaultImplOfLookup()
        {
            var needs = new Needs();
            var thing = needs.Get<ISomeInterface>().OrElseThrow(() => new Exception("No impl found"));
            Assert.IsInstanceOfType(thing, typeof(SomeImplementation));
        }

        public interface ISomeInterface { }

        [DefaultImplementationOf(typeof(ISomeInterface))]
        public class SomeImplementation : ISomeInterface { }

        [TestMethod]
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

        [TestMethod]
        public void NoImplAvailable()
        {
            var needs = new Needs();
            Expect.None(needs.Get<IOtherInterface>());
        }

        public interface IOtherInterface { }
    }

}
