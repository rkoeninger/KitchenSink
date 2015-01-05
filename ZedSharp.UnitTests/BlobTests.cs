using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class BlobTests
    {
        private static IEnumerable<Blob<int>> Blobs()
        {
            yield return default(Blob<int>);
            yield return Blob.Of<int>();
            yield return Blob.Of(0);
            yield return Blob.Of(1);
            yield return Blob.Of(1, 2);
            yield return Blob.Of(1, 2, 3);

            foreach (var len in Seq.Forever(() => Rand.Int(32)).Take(32))
                yield return Blob.Of(Rand.Ints().Take(len));
        }

        [TestMethod]
        public void BlobProperties()
        {
            Check.EqualsAndHashCode(Blobs());
            Check.ReflexiveEquality(Blobs());
            Check.That((x, y) => Equals(x, y).Implies(Blob.Compare(x, y) == 0), Blobs(), Blobs());
            Check.That((x, y) => Blob.Compare(y, x) == -Blob.Compare(x, y), Blobs(), Blobs());
        }
    }
}
