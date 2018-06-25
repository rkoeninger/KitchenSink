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
        public void SubdividedRetryPartialSuccess()
        {
            var i = 0;
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
            var committedCounts = ListOf<int>();

            void attempt(IReadOnlyList<int> xs)
            {
                if (i >= plan.Count || !plan[i++])
                {
                    throw new Exception(i.ToString());
                }

                committedCounts.Add(xs.Count);
            }

            var result = Retry.Subdivide(
                2,
                4,
                1.ToIncluding(65536),
                attempt,
                e => true);
            Assert.AreEqual(committedCounts.Sum(), result.SuccessCount);
            Assert.IsTrue(committedCounts.SequenceEqual(SeqOf(4096, 4096, 4096, 4096, 16384, 4096)));
            Assert.IsTrue(result.HasError);
            Assert.AreEqual(plan.Count.ToString(), result.Error.Message);
        }
    }
}
