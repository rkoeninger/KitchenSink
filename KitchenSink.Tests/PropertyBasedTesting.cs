using System;
using System.Linq;
using static KitchenSink.Operators;
using KitchenSink.Testing;
using KitchenSink.Timekeeping;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    public class PropertyBasedTesting
    {
        [Test]
        public void CheckWithDefaultInputs()
        {
            Check.That((int x, int y) => (x == y) == (y == x));
            Check.ReflexiveEquality<int>();
        }

        [Test]
        public void CheckWithProvidedInputs()
        {
            Check.That(
                Sample.Ints.Except(SeqOf(int.MinValue)),
                x => Math.Abs(x) >= 0);

            Check.EqualsAndHashCode(SeqOf(
                ExactDay.On(2001, 4, 13),
                ExactDay.On(1947, 1, 20),
                ExactDay.On(2030, 11, 9),
                ExactDay.On(1994, 9, 16),
                ExactDay.On(2023, 8, 27)));

            Check.EqualsAndHashCode(Forever(() => Rand.Ints().Take(Rand.Int(64))).Take(16));
        }

        [Test]
        public void CheckComparableAndOperators()
        {
            Check.Comparable(Sample.Ints);
            Check.CompareOperators(Sample.Ints);
        }

        [Test]
        public void Idempotence()
        {
            Check.Idempotent(Sample.Ints.Where(x => x != int.MinValue), Math.Abs);
            Expect.FailedAssert(() => Check.Idempotent(Inc));
        }
    }
}
