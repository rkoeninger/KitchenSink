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
        private static readonly String path = "Test.csconfig";
        private static readonly CodeLoader<IConfig> loader = new CodeLoader<IConfig>(path);
        public static void LoadConfig() { loader.Load(); }
        public static IConfig Config { get { return loader.Value; } }
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
