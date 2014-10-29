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
        }
    }
}
