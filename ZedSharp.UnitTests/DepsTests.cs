using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class DepsTests
    {
        [TestMethod]
        public void DepsLookup()
        {
            var x = new WS();
            var y = new DbRW();
            var z = new DbR();
            var w = new UI();

            var deps = Deps.Of(x, y, z, w);

            Assert.AreEqual(y, deps.GetOrThrow<IDatabaseQuery>());
            Assert.AreEqual(y, deps.GetOrThrow<IDatabaseCommand>());
            Assert.AreEqual(w, deps.GetOrThrow<IUserInterface>());
            Assert.AreEqual(x, deps.GetOrThrow<IWebService>());
        }
    }

    class DbRW : IDatabaseQuery, IDatabaseCommand { }

    class DbR : IDatabaseQuery { }

    class UI : IUserInterface { }

    class WS : IWebService { }

    public interface IDatabaseQuery { }

    public interface IDatabaseCommand { }

    public interface IUserInterface { }

    public interface IWebService { }
}
