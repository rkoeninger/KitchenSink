using System;
using System.Collections.Generic;
using System.Linq;
using KitchenSink.Extensions;
using static KitchenSink.Operators;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    public class Reliability
    {
        [Test]
        public void FractalImmediateSuccess()
        {
            var plan = ListOf(true); // whole batch succeeds
            var commits = ListOf<int>();
            var (count, error) = Retry.Fractal(
                2,
                4,
                1.ToIncluding(65536),
                Attempt<int>(plan, commits),
                e => true);
            Assert.AreEqual(65536, count);
            Assert.AreEqual(65536, commits.Sum());
            Assert.AreEqual(1, commits.Count);
            Assert.IsNull(error);
        }

        [Test]
        public void FractalCompleteFailure()
        {
            var plan = ListOf(
                false, // subdivide into 4 groups of 16k
                false, // subdivide into 4 groups of 4k
                false); // final failure
            var commits = ListOf<int>();
            var (count, error) = Retry.Fractal(
                2,
                4,
                1.ToIncluding(65536),
                Attempt<int>(plan, commits),
                e => true);
            Assert.AreEqual(0, count);
            Assert.AreEqual(0, commits.Sum());
            Assert.IsNotNull(error);
            Assert.AreEqual(plan.Count.ToString(), error.Message);
        }

        [Test]
        public void FractalPartialSuccess()
        {
            var plan = ListOf(
                false, // subdivide into 4 groups of 16k
                false, // subdivide into 4 groups of 4k
                true, // 4k successful
                true, // 4k successful
                true, // 4k successful
                true, // 4k successful
                true, // 16k successful
                false, // subdivide into 4 groups of 4k
                true, // 4k successful
                false); // final failure
            var commits = ListOf<int>();
            var (count, error) = Retry.Fractal(
                2,
                4,
                1.ToIncluding(65536),
                Attempt<int>(plan, commits),
                e => true);
            Assert.AreEqual(36864, count);
            Assert.AreEqual(36864, commits.Sum());
            Assert.IsTrue(commits.SequenceEqual(SeqOf(4096, 4096, 4096, 4096, 16384, 4096)));
            Assert.IsNotNull(error);
            Assert.AreEqual(plan.Count.ToString(), error.Message);
        }

        [Test]
        public void FractalEventualSuccess()
        {
            var plan = ListOf(
                false, // subdivide into 4 groups of 16k
                false, // subdivide into 4 groups of 4k
                true, // 4k successful
                true, // 4k successful
                true, // 4k successful
                true, // 4k successful
                true, // 16k successful
                false, // subdivide into 4 groups of 4k
                true, // 4k successful
                true, // 4k successful
                true, // 4k successful
                true, // 4k successful
                true); // 16k successful
            var commits = ListOf<int>();
            var (count, error) = Retry.Fractal(
                2,
                4,
                1.ToIncluding(65536),
                Attempt<int>(plan, commits),
                e => true);
            Assert.AreEqual(65536, count);
            Assert.AreEqual(65536, commits.Sum());
            Assert.IsTrue(commits.SequenceEqual(SeqOf(4096, 4096, 4096, 4096, 16384, 4096, 4096, 4096, 4096, 16384)));
            Assert.IsNull(error);
        }

        private static Action<IReadOnlyList<A>> Attempt<A>(IReadOnlyList<bool> plan, ICollection<int> commits)
        {
            var i = 0;

            return xs =>
            {
                if (i < plan.Count && !plan[i++])
                {
                    throw new Exception(i.ToString());
                }

                commits.Add(xs.Count);
            };
        }
    }
}
