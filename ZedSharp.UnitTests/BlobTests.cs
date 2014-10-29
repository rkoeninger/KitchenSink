using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class BlobTests
    {
        [TestMethod]
        public void BlobEquality()
        {
            Assert.IsTrue(Blob.Of(1, 2, 3) == Blob.Of(1, 2, 3));
            Assert.IsTrue(default(Blob<int>) == Blob.Of<int>());
        }

        [TestMethod]
        public void BlobComparison()
        {
            Assert.IsTrue(Blob.Compare(Blob.Of(1, 2, 3), Blob.Of(1, 2, 3)) == 0);
            Assert.IsTrue(Blob.Compare(Blob.Of<int>(), Blob.Of<int>()) == 0);
            Assert.IsTrue(Blob.Compare(Blob.Of(1, 2), Blob.Of(1, 2, 3)) < 0);
            Assert.IsTrue(Blob.Compare(Blob.Of(1, 2), Blob.Of<int>()) > 0);
            Assert.IsTrue(Blob.Compare(Blob.Of(7, 3, 5), Blob.Of(7, 4, 3)) < 0);
            Assert.IsTrue(Blob.Compare(default(Blob<String>), Blob.Of("hi")) < 0);
        }
    }
}
