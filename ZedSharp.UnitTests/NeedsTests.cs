﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class NeedsTests
    {
        [TestMethod]
        public void NeedsLookup()
        {
            var x = new WS();
            var y = new DbW();
            var z = new DbR();
            var w = new UI();

            var deps = new Needs();
            deps.Set<IWebService>(x);
            deps.Set<IDatabaseCommand>(y);
            deps.Set<IDatabaseQuery>(z);
            deps.Set<IUserInterface>(w);

            Expect.Some(z, deps.Get<IDatabaseQuery>());
            Expect.Some(y, deps.Get<IDatabaseCommand>());
            Expect.Some(w, deps.Get<IUserInterface>());
            Expect.Some(x, deps.Get<IWebService>());
        }

        [TestMethod]
        public void NeedsDefaultImplLookup()
        {
            var clock = new Needs().Get<IClock>().OrElseThrow(() => new Exception("No clock impl found"));

            // internal protection level, so we can't refer to typeof(LiveClock)
            Assert.IsTrue(clock.GetType().Name.Contains("LiveClock"));
        }
    }

    class DbW : IDatabaseCommand { }
    class DbR : IDatabaseQuery { }
    class UI : IUserInterface { }
    class WS : IWebService { }
    public interface IDatabaseQuery { }
    public interface IDatabaseCommand { }
    public interface IUserInterface { }
    public interface IWebService { }
}