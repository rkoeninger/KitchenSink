using System;
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

        [Test]
        public void SuccessfulExplicitTran()
        {
            var stm = new Stm();
            var ref1 = stm.NewRef(0);
            var ref2 = stm.NewRef("a");
            var ref3 = stm.NewRef(false);

            using (var tran = stm.BeginTran())
            {
                ref1.Update(tran, x => x + 1);
                ref2.Update(tran, x => x + "b");
                ref3.Update(tran, x => true);
            }

            Assert.AreEqual(1, ref1.Value);
            Assert.AreEqual("ab", ref2.Value);
            Assert.IsTrue(ref3.Value);
        }

        [Test]
        public void FailedExplicitTran()
        {
            var stm = new Stm();
            var ref1 = stm.NewRef(0);
            var ref2 = stm.NewRef("a");
            var ref3 = stm.NewRef(false);

            Assert.Throws<SomeException>(() =>
            {
                using (var tran = stm.BeginTran())
                {
                    ref1.Update(tran, x => x + 1);
                    ref2.Update(tran, x => x + "b");
                    ref3.Update(tran, x => throw new SomeException());
                }
            });

            Assert.AreEqual(0, ref1.Value);
            Assert.AreEqual("a", ref2.Value);
            Assert.IsFalse(ref3.Value);
        }

        [Test]
        public void SuccessfulAmbientTran()
        {
            var stm = new Stm();
            var ref1 = stm.NewRef(0);
            var ref2 = stm.NewRef("a");
            var ref3 = stm.NewRef(false);

            using (stm.BeginTran(true))
            {
                ref1.Update(x => x + 1);
                ref2.Update(x => x + "b");
                ref3.Update(x => true);
            }

            Assert.AreEqual(1, ref1.Value);
            Assert.AreEqual("ab", ref2.Value);
            Assert.IsTrue(ref3.Value);
        }

        [Test]
        public void FailedAmbientTran()
        {
            var stm = new Stm();
            var ref1 = stm.NewRef(0);
            var ref2 = stm.NewRef("a");
            var ref3 = stm.NewRef(false);

            Assert.Throws<SomeException>(() =>
            {
                using (stm.BeginTran(true))
                {
                    ref1.Update(x => x + 1);
                    ref2.Update(x => x + "b");
                    ref3.Update(x => throw new SomeException());
                }
            });

            Assert.AreEqual(0, ref1.Value);
            Assert.AreEqual("a", ref2.Value);
            Assert.IsFalse(ref3.Value);
        }

        [Test]
        public void TentativeRefValueInTran()
        {
            var stm = new Stm();
            var r = stm.NewRef(0);

            Assert.Throws<SomeException>(() =>
            {
                stm.InTran(() =>
                {
                    r.Update(Inc);
                    Assert.AreEqual(1, r.Value);
                    throw new SomeException();
                });
            });

            Assert.AreEqual(0, r.Value);
        }

        [Test]
        public void RefAssignment()
        {
            var stm = new Stm();
            var r = stm.NewRef(0);
            r.Value = 1;
            Assert.AreEqual(1, r.Value);
        }

        [Test]
        public void NestedTrans()
        {
            var stm = new Stm();
            var r = stm.NewRef(0);

            stm.InTran(() =>
            {
                Assert.AreEqual(0, r.Value);
                r.Update(Inc);
                Assert.AreEqual(1, r.Value);

                stm.InTran(() =>
                {
                    Assert.AreEqual(1, r.Value);
                    r.Update(Inc);
                    Assert.AreEqual(2, r.Value);
                });

                Assert.AreEqual(2, r.Value);
                r.Update(Inc);
                Assert.AreEqual(3, r.Value);
            });

            Assert.AreEqual(3, r.Value);
        }

        [Test]
        public void NestedTranFailure()
        {
            var stm = new Stm();
            var r = stm.NewRef(0);

            stm.InTran(() =>
            {
                Assert.AreEqual(0, r.Value);
                r.Update(Inc);
                Assert.AreEqual(1, r.Value);

                Assert.Throws<SomeException>(() =>
                {
                    stm.InTran(() =>
                    {
                        Assert.AreEqual(1, r.Value);
                        r.Update(Inc);
                        Assert.AreEqual(2, r.Value);
                        throw new SomeException();
                    });
                });

                Assert.AreEqual(1, r.Value);
                r.Update(Inc);
                Assert.AreEqual(2, r.Value);
            });

            Assert.AreEqual(2, r.Value);
        }

        [Test]
        public void UncommitedValuesNotVisibleOutsideTran()
        {
            var stm = new Stm();
            var r = stm.NewRef(0);
            var tran = stm.BeginTran();
            r.Update(tran, Inc);
            Assert.AreEqual(0, r.Value);
            tran.Dispose();
            Assert.AreEqual(1, r.Value);
        }

        public class SomeException : Exception
        {
        }
    }
}
