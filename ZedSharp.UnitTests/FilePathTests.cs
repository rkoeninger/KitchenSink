using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class FilePathTests
    {
        [TestMethod]
        public void ConcatPaths()
        {
            Assert.AreEqual(
                @"C:\Program Files (x86)\Microsoft Visual Studio 2013",
                Drive.C / "Program Files (x86)" / "Microsoft Visual Studio 2013");

            Assert.AreEqual(
                @"\\somemachine\someshare\subpath\file.txt",
                UNC.Host("somemachine").Share("someshare") / "subpath" / "file.txt");
        }
    }
}
