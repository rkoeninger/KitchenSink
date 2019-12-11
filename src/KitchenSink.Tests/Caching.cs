using System;
using NUnit.Framework;
using static KitchenSink.Operators;
using System.Threading;

namespace KitchenSink.Tests
{
    public class Caching
    {
        private static readonly Random Rand = new Random();
        private static readonly Func<string, int> G = _ => Rand.Next();

        [Test]
        public void FunctionMemoization()
        {
            var f = Memo(G);
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

        private static int _callCount;

        public class UserRepository : IUserRepostiory
        {
            public string Get(int id) => Guid.NewGuid().ToString();
            public string Get2(int id, string x) => Guid.NewGuid().ToString();
            public string Get0() => Guid.NewGuid().ToString();
            public void Do(int id) => _callCount++;
        }

        [Test, Ignore("Cache() not working, see AutoCache.cs")]
        public void AutoCaching()
        {
            IUserRepostiory repo = new UserRepository();
            var cachedRepo = Cache(repo);
            Assert.AreEqual(cachedRepo.Get(1), cachedRepo.Get(1));
            Assert.AreEqual(cachedRepo.Get(2), cachedRepo.Get(2));
            Assert.AreEqual(cachedRepo.Get(3), cachedRepo.Get(3));

            var prevCallCount = _callCount;
            cachedRepo.Do(0);
            Assert.AreEqual(prevCallCount + 1, _callCount);

            Assert.AreEqual(cachedRepo.Get0(), cachedRepo.Get0());
            Assert.AreEqual(cachedRepo.Get0(), cachedRepo.Get0());
            Assert.AreEqual(cachedRepo.Get0(), cachedRepo.Get0());

            Assert.AreEqual(cachedRepo.Get2(1, "abc"), cachedRepo.Get2(1, "abc"));
            Assert.AreEqual(cachedRepo.Get2(2, "def"), cachedRepo.Get2(2, "def"));
            Assert.AreEqual(cachedRepo.Get2(3, "ghi"), cachedRepo.Get2(3, "ghi"));
        }

        [Test, Ignore("Cache() not working, see AutoCache.cs")]
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

        [Test, Ignore("Cache() not working, see AutoCache.cs")]
        public void ConfigSpecificCachiingOfGeneratedTypes()
        {
            // Run other tests again to confirm caching works as it should
            SelectiveExclusion();
            AutoCaching();
        }

        [Test, Ignore("Cache() not working, see AutoCache.cs")]
        public void ExpirableValuesAreRegenerated()
        {
            IUserRepostiory repo = new UserRepository();
            var cachedRepo = Cache(repo, c => c.Expire(TimeSpan.FromMilliseconds(100), m => m.Get0()));
            var prev = cachedRepo.Get0();
            Assert.AreEqual(prev, cachedRepo.Get0());
            Assert.AreEqual(prev, cachedRepo.Get0());
            Assert.AreEqual(prev, cachedRepo.Get0());
            Thread.Sleep(150);
            Assert.AreNotEqual(prev, cachedRepo.Get0());
        }
    }
}
