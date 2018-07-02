using System;
using System.Linq;
using System.Threading.Tasks;
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
                    .Select(Mask(ValueMask))
                    .Take(ListLength)
                    .ToList()
                ).ToList();
            var total = lists.Sum(Sum);
            var tasks = lists.Select(xs =>
                Task.Run(() =>
                    xs.ForEach(x => atom.Update(y => y + x))
                )).ToArray();
            Task.WaitAll(tasks);
            Assert.AreEqual(total, atom.Value);
        }

        [Test]
        public void AtomAsyncAtomicity()
        {
            var atom = Atom.Of(0);
            var lists = Repeatedly(ListCount, () =>
                Rand.Ints()
                    .Select(Mask(ValueMask))
                    .Take(ListLength)
                    .ToList()
                ).ToList();
            var total = lists.Sum(Sum);
            var tasks = lists.Select(xs =>
                Task.Run(() =>
                    Task.WaitAll(xs
                        .Select(x => (Task)atom.UpdateAsync(y => y + x))
                        .ToArray()))).ToArray();
            Task.WaitAll(tasks);
            Assert.AreEqual(total, atom.Value);
        }

        [Test]
        public void AtomZippedAtomicity()
        {
            var atomA = Atom.Of(0);
            var atomB = Atom.Of(0);
            var atomC = Atom.Of(0);
            var atomD = Atom.Of(0);
            var atomA2 = Atom.Zip(
                atomA,
                atomB,
                atomC,
                atomD,
                (a_, b_, c_, d_) => a_,
                x => (x, atomB.Value, atomC.Value, atomD.Value));
            var atomB2 = Atom.Zip(
                atomA,
                atomB,
                atomC,
                atomD,
                (a_, b_, c_, d_) => b_,
                x => (atomA.Value, x, atomC.Value, atomD.Value));
            var atomC2 = Atom.Zip(
                atomA,
                atomB,
                atomC,
                atomD,
                (a_, b_, c_, d_) => c_,
                x => (atomA.Value, atomB.Value, x, atomD.Value));
            var atomD2 = Atom.Zip(
                atomA,
                atomB,
                atomC,
                atomD,
                (a_, b_, c_, d_) => d_,
                x => (atomA.Value, atomB.Value, atomC.Value, x));
            var lists = Repeatedly(ListCount, () =>
                Rand.Ints()
                    .Select(Mask(ValueMask))
                    .Take(ListLength)
                    .ToList()
            ).ToList();
            var total = lists.Sum(Sum);
            var tasks = lists.Select(xs =>
                Task.Run(() =>
                    xs.ForEach(x =>
                    {
                        switch (Abs(x) % 4)
                        {
                            case 0:
                                atomA2.Update(y => y + x);
                                break;
                            case 1:
                                atomB2.Update(y => y + x);
                                break;
                            case 2:
                                atomC2.Update(y => y + x);
                                break;
                            case 3:
                                atomD2.Update(y => y + x);
                                break;
                            default:
                                throw new ArithmeticException();
                        }
                    })
                )).ToArray();
            Task.WaitAll(tasks);
            Assert.AreEqual(total, atomA.Value + atomB.Value + atomC.Value + atomD.Value);
        }

        [Test]
        public void AtomFocus()
        {
            var atomTuple = Atom.Of((1, "a"));
            var atomInt = atomTuple.Focus(x => x.Item1, (t, x) => (x, t.Item2));
            var atomString = atomTuple.Focus(x => x.Item2, (t, y) => (t.Item1, y));
            Assert.AreEqual((1, "a"), atomTuple.Value);
            atomInt.Update(Inc);
            Assert.AreEqual((2, "a"), atomTuple.Value);
            atomString.Update(x => x + "b");
            Assert.AreEqual((2, "ab"), atomTuple.Value);
            atomTuple.Update(t =>
            {
                var (x, y) = t;
                return (x + 1, y + "c");
            });
            Assert.AreEqual((3, "abc"), atomTuple.Value);
            Assert.AreEqual(3, atomInt.Value);
            Assert.AreEqual("abc", atomString.Value);
        }

        [Test]
        public void AtomZipping()
        {
            var atomA = Atom.Of("abc");
            var atomB = Atom.Of("def");
            var atomZ = Atom.Zip(
                atomA,
                atomB,
                (x, y) => x + '|' + y,
                x => (x.Split('|')[0], x.Split('|')[1]));
            Assert.AreEqual("abc|def", atomZ.Value);
            atomA.Value = "xyz";
            Assert.AreEqual("xyz|def", atomZ.Value);
            atomZ.Update(x => new string(x.Reverse().ToArray()));
            Assert.AreEqual("fed|zyx", atomZ.Value);
            Assert.AreEqual("fed", atomA.Value);
            Assert.AreEqual("zyx", atomB.Value);
        }

        public class SomeException : Exception
        {
        }
    }
}
