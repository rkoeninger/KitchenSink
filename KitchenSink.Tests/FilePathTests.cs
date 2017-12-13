using System;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    [TestFixture]
    public class FilePathTests
    {
        [Test, Category("WindowsOnly")]
        public void ConcatPaths()
        {
            Assert.AreEqual(
                @"C:\Program Files (x86)\Microsoft Visual Studio 2017",
                (Drive.C / "Program Files (x86)" / "Microsoft Visual Studio 2017").Value);

            Assert.AreEqual(
                @"\\somemachine\someshare\subpath\file.txt",
                (UNC.Host("somemachine").Share("someshare") / "subpath" / "file.txt").Value);
        }

        [Test, Category("WindowsOnly")]
        public void TryOutSpecialDrives()
        {
            Console.WriteLine(Drive.System);
        }

        [Test]
        public void TryOutSpecialFolders()
        {
            Console.WriteLine($"{nameof(Folder.AppData)}      = {Folder.AppData}");
            Console.WriteLine($"{nameof(Folder.LocalAppData)} = {Folder.LocalAppData}");
            Console.WriteLine($"{nameof(Folder.Desktop)}      = {Folder.Desktop}");
            Console.WriteLine($"{nameof(Folder.Documents)}    = {Folder.Documents}");
            Console.WriteLine($"{nameof(Folder.Profile)}      = {Folder.Profile}");
            Console.WriteLine($"{nameof(Folder.Current)}      = {Folder.Current}");
            Console.WriteLine($"{nameof(Folder.Programs)}     = {Folder.Programs}");
        }
    }
}
