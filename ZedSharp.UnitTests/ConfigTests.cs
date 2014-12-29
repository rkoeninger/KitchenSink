using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    public interface IConfig
    {
        String BrandName { get; }

        String FormatName(String name);
    }

    public static class App
    {
        private const String Path = "Test.csconfig";
        private static readonly CodeLoader<IConfig> Loader = new CodeLoader<IConfig>(Path);
        public static void LoadConfig() { Loader.Load(); }
        public static IConfig Config { get { return Loader.Value; } }
    }

    [TestClass]
    public class ConfigTests
    {
        [TestMethod]
        public void TestConfigLoading()
        {
            App.LoadConfig();
            Assert.AreEqual("Brand Name", App.Config.BrandName);
            Assert.AreEqual("BOB", App.Config.FormatName("bob"));
        }
    }
}
