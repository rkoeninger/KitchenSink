using System;
using NUnit.Framework;

namespace ZedSharp.UnitTests
{
    [TestFixture]
    public class FilePathTests
    {
        [Test]
        public void ConcatPaths()
        {
            Assert.AreEqual(
                @"C:\Program Files (x86)\Microsoft Visual Studio 2013",
                (Drive.C / "Program Files (x86)" / "Microsoft Visual Studio 2013").Value);

            Assert.AreEqual(
                @"\\somemachine\someshare\subpath\file.txt",
                (UNC.Host("somemachine").Share("someshare") / "subpath" / "file.txt").Value);
        }

        [Test]
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
