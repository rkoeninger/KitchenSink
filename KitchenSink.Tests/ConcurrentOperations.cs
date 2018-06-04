using System.Linq;
using System.Threading.Tasks;
using KitchenSink.Concurrent;
using static KitchenSink.Operators;
using KitchenSink.Testing;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    public class ConcurrentOperations
    {
        private const int ListCount = 100;
        private const int ListLength = 1000;
        private const int ValueMask = 0xff;

        [Test]
        public void AtomAtomicity()
        {
            var atom = Atom.Of(0);
            var lists = Repeatedly(ListCount, () =>
                Rand.Ints()
                    .Select(x => x & ValueMask)
                    .Take(ListLength)
                    .ToList()
                ).ToList();
            var total = lists.Sum(xs => xs.Sum());
            var tasks = lists.Select(xs =>
                Task.Run(() =>
                    xs.ForEach(x => atom.Update(Curry(Add)(x)))
                )).ToArray();
            Task.WaitAll(tasks);
            Assert.AreEqual(total, atom.Value);
        }

        [Test]
        public void AgentAtomicity()
        {
            var agent = Agent.Of(0);
            var lists = Repeatedly(ListCount, () =>
                Rand.Ints()
                    .Select(x => x & ValueMask)
                    .Take(ListLength)
                    .ToList()
                ).ToList();
            var total = lists.Sum(xs => xs.Sum());
            var tasks = lists.Select(xs =>
                Task.Run(() =>
                    Task.WaitAll(xs
                        .Select(x => (Task)agent.Update(Curry(Add)(x)))
                        .ToArray()))).ToArray();
            Task.WaitAll(tasks);
            Assert.AreEqual(total, agent.Value);
        }
    }
}
