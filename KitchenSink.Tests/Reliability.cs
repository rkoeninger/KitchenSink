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
        public void SubdivideImmediateSuccess()
        {
            var plan = ListOf(true); // whole batch succeeds
            var commits = ListOf<int>();
            var result = Retry.Subdivide(
                2,
                4,
                1.ToIncluding(65536),
                Attempt<int>(plan, commits),
                e => true);
            Assert.AreEqual(65536, result.SuccessCount);
            Assert.AreEqual(65536, commits.Sum());
            Assert.AreEqual(1, commits.Count);
            Assert.IsFalse(result.HasError);
        }

        [Test]
        public void SubdivideCompleteFailure()
        {
            var plan = ListOf(
                false, // subdivide into 4 groups of 16k
                false, // subdivide into 4 groups of 4k
                false); // final failure
            var commits = ListOf<int>();
            var result = Retry.Subdivide(
                2,
                4,
                1.ToIncluding(65536),
                Attempt<int>(plan, commits),
                e => true);
            Assert.AreEqual(0, result.SuccessCount);
            Assert.AreEqual(0, commits.Sum());
            Assert.IsTrue(result.HasError);
            Assert.AreEqual(plan.Count.ToString(), result.Error.Message);
        }

        [Test]
        public void SubdividePartialSuccess()
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
            var result = Retry.Subdivide(
                2,
                4,
                1.ToIncluding(65536),
                Attempt<int>(plan, commits),
                e => true);
            Assert.AreEqual(36864, result.SuccessCount);
            Assert.AreEqual(36864, commits.Sum());
            Assert.IsTrue(commits.SequenceEqual(SeqOf(4096, 4096, 4096, 4096, 16384, 4096)));
            Assert.IsTrue(result.HasError);
            Assert.AreEqual(plan.Count.ToString(), result.Error.Message);
        }

        [Test]
        public void SubdivideEventualSuccess()
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
            var result = Retry.Subdivide(
                2,
                4,
                1.ToIncluding(65536),
                Attempt<int>(plan, commits),
                e => true);
            Assert.AreEqual(65536, result.SuccessCount);
            Assert.AreEqual(65536, commits.Sum());
            Assert.IsTrue(commits.SequenceEqual(SeqOf(4096, 4096, 4096, 4096, 16384, 4096, 4096, 4096, 4096, 16384)));
            Assert.IsFalse(result.HasError);
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
