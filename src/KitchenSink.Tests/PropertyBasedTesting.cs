using System;
using System.Linq;
using static KitchenSink.Operators;
using KitchenSink.Timekeeping;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    public class PropertyBasedTesting
    {
        [Test]
        public void CheckWithDefaultInputs()
        {
            Expect.That((int x, int y) => (x == y) == (y == x));
            Expect.ReflexiveEquality<int>();
        }

        [Test]
        public void CheckWithProvidedInputs()
        {
            Expect.That(
                Sample.Ints.Except(SeqOf(int.MinValue)),
                x => Math.Abs(x) >= 0);

            Expect.EqualsAndHashCode(SeqOf(
                ExactDay.On(2001, 4, 13),
                ExactDay.On(1947, 1, 20),
                ExactDay.On(2030, 11, 9),
                ExactDay.On(1994, 9, 16),
                ExactDay.On(2023, 8, 27)));

            Expect.EqualsAndHashCode(Forever(() => Rand.Ints().Take(Rand.Int(64))).Take(16));
        }

        [Test]
        public void CheckComparableAndOperators()
        {
            Expect.Comparable(Sample.Ints);
            // TODO: Expect.CompareOperators can't be built in .net standard
            //Expect.CompareOperators(Sample.Ints);
        }

        [Test]
        public void Idempotence()
        {
            Expect.Idempotent(Sample.Ints.Where(x => x != int.MinValue), Math.Abs);
            Expect.FailedAssert(() => Expect.Idempotent(Inc));
        }
    }
}
