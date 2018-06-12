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
            //string Get2(int id, string x);
            //void Do(int id);
        }

        public class UserRepository : IUserRepostiory
        {
            public string Get(int id) => Rand.AsciiString(16);
            //public string Get2(int id, string x) => Rand.AsciiString(32);
            //public void Do(int id) {}
        }

        public class CachedUserRepository : IUserRepostiory
        {
            private readonly IUserRepostiory _inner;
            private readonly ConcurrentDictionary<int, string> _cache0 = new ConcurrentDictionary<int, string>();
            //private readonly ConcurrentDictionary<(int, string), string> _cache1 = new ConcurrentDictionary<(int, string), string>();

            public CachedUserRepository(IUserRepostiory inner) => _inner = inner;

            public string Get(int id) => _cache0.GetOrAdd(id, _inner.Get);
            //public string Get2(int id, string x) => _cache1.GetOrAdd((id, x), t => _inner.Get2(t.Item1, t.Item2));
            //public void Do(int id) => _inner.Do(id);
        }

        [Test]
        public void AutoCaching()
        {
            IUserRepostiory repo = new UserRepository();
            var cachedRepo = Cache(repo);
            Assert.AreEqual(cachedRepo.Get(1), cachedRepo.Get(1));
            Assert.AreEqual(cachedRepo.Get(2), cachedRepo.Get(2));
            Assert.AreEqual(cachedRepo.Get(3), cachedRepo.Get(3));
        }
    }
}
