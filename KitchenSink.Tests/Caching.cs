using System;
using NUnit.Framework;
using static KitchenSink.Operators;
using KitchenSink.Testing;

namespace KitchenSink.Tests
{
    public class Caching
    {
        private static readonly Random rand = new Random();
        private static readonly Func<string, int> g = _ => rand.Next();

        [Test]
        public void FunctionMemoization()
        {
            var f = Memo(g);
            Assert.AreEqual(f("a"), f("a"));
            Assert.AreEqual(f("b"), f("b"));
            Assert.AreEqual(f("c"), f("c"));
        }

        public interface IUserRepostiory
        {
            string Get(int id);
            string Get2(int id, string x);
            string Get0();
            void Do(int id);
        }

        private static bool doCalled;

        public class UserRepository : IUserRepostiory
        {
            public string Get(int id) => Rand.AsciiString(16);
            public string Get2(int id, string x) => Rand.AsciiString(32);
            public string Get0() => Rand.AsciiString(8);
            public void Do(int id) => doCalled = true;
        }

        [Test]
        public void AutoCaching()
        {
            IUserRepostiory repo = new UserRepository();
            var cachedRepo = Cache(repo);
            Assert.AreEqual(cachedRepo.Get(1), cachedRepo.Get(1));
            Assert.AreEqual(cachedRepo.Get(2), cachedRepo.Get(2));
            Assert.AreEqual(cachedRepo.Get(3), cachedRepo.Get(3));

            Assert.IsFalse(doCalled);
            cachedRepo.Do(0);
            Assert.IsTrue(doCalled);

            Assert.AreEqual(cachedRepo.Get0(), cachedRepo.Get0());

            Assert.AreEqual(cachedRepo.Get2(1, "abc"), cachedRepo.Get2(1, "abc"));
            Assert.AreEqual(cachedRepo.Get2(2, "def"), cachedRepo.Get2(2, "def"));
            Assert.AreEqual(cachedRepo.Get2(3, "ghi"), cachedRepo.Get2(3, "ghi"));
        }

        [Test]
        public void SelectiveExclusion()
        {
            IUserRepostiory repo = new UserRepository();
            var cachedRepo = Cache(repo, c => c.Exclude(m => m.Get2(default, default)));

            Assert.AreEqual(cachedRepo.Get(1), cachedRepo.Get(1));
            Assert.AreEqual(cachedRepo.Get(2), cachedRepo.Get(2));
            Assert.AreEqual(cachedRepo.Get(3), cachedRepo.Get(3));

            Assert.AreNotEqual(cachedRepo.Get2(1, "abc"), cachedRepo.Get2(1, "abc"));
            Assert.AreNotEqual(cachedRepo.Get2(2, "def"), cachedRepo.Get2(2, "def"));
            Assert.AreNotEqual(cachedRepo.Get2(3, "ghi"), cachedRepo.Get2(3, "ghi"));
        }
    }
}
