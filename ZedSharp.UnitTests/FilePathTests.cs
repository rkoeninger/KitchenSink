using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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

        [TestMethod]
        public void TryOutSpecialFolders()
        {
            Console.WriteLine(Folder.AppData);
            Console.WriteLine(Folder.LocalAppData);
            Console.WriteLine(Folder.Desktop);
            Console.WriteLine(Folder.Documents);
            Console.WriteLine(Folder.Profile);
            Console.WriteLine(Folder.Current);
            Console.WriteLine(Drive.System);
            Console.WriteLine(Folder.Programs);
        }
    }
}
