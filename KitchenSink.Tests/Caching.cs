using System;
using System.Collections.Concurrent;
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
            int GetB(string id);
            //string Get2(int id, string x);
            string Get0();
            void Do(int id);
        }

        private static bool doCalled;

        public class UserRepository : IUserRepostiory
        {
            public string Get(int id) => Rand.AsciiString(16);
            public int GetB(string id) => Rand.Int();
            //public string Get2(int id, string x) => Rand.AsciiString(32);
            public string Get0() => Rand.AsciiString(8);
            public void Do(int id) => doCalled = true;
        }

        public class CachedUserRepository : IUserRepostiory
        {
            private readonly IUserRepostiory _inner;
            private readonly ConcurrentDictionary<int, string> _cache0;
            private readonly ConcurrentDictionary<string, int> _cache5;
            //private readonly ConcurrentDictionary<(int, string), string> _cache1;
            private readonly Lazy<string> _cache2;

            public CachedUserRepository(IUserRepostiory inner)
            {
                _inner = inner;
                _cache0 = new ConcurrentDictionary<int, string>();
                _cache5 = new ConcurrentDictionary<string, int>();
                //_cache1 = new ConcurrentDictionary<(int, string), string>();
                _cache2 = new Lazy<string>(_inner.Get0);
            }

            public string Get(int id) => _cache0.GetOrAdd(id, _inner.Get);
            public int GetB(string id) => _cache5.GetOrAdd(id, _inner.GetB);
            //public string Get2(int id, string x) => _cache1.GetOrAdd((id, x), t => _inner.Get2(t.Item1, t.Item2));
            public void Do(int id) => _inner.Do(id);
            public string Get0() => _cache2.Value;
        }

        [Test]
        public void AutoCaching()
        {
            IUserRepostiory repo = new UserRepository();
            var cachedRepo = Cache(repo);
            Assert.AreEqual(cachedRepo.Get(1), cachedRepo.Get(1));
            Assert.AreEqual(cachedRepo.Get(2), cachedRepo.Get(2));
            Assert.AreEqual(cachedRepo.Get(3), cachedRepo.Get(3));

            Assert.AreEqual(cachedRepo.GetB("abc"), cachedRepo.GetB("abc"));
            Assert.AreEqual(cachedRepo.GetB("def"), cachedRepo.GetB("def"));
            Assert.AreEqual(cachedRepo.GetB("ghi"), cachedRepo.GetB("ghi"));

            //Assert.AreEqual(cachedRepo.Get2(1, "abc"), cachedRepo.Get2(1, "abc"));
            //Assert.AreEqual(cachedRepo.Get2(2, "def"), cachedRepo.Get2(2, "def"));
            //Assert.AreEqual(cachedRepo.Get2(3, "ghi"), cachedRepo.Get2(3, "ghi"));

            Assert.IsFalse(doCalled);
            cachedRepo.Do(0);
            Assert.IsTrue(doCalled);

            Assert.AreEqual(cachedRepo.Get0(), cachedRepo.Get0());
        }
    }
}
