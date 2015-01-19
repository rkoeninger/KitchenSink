using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    public interface IConfig
    {
        String BrandName { get; }

        String BrandNameWithTradeMark { get; }

        String FormatName(String name);
    }

    public static class App
    {
        private const String Path = @"
            using System;

            public class Config : ZedSharp.UnitTests.IConfig
            {
	            public String BrandName { get { return ""Brand Name""; } }

                public String BrandNameWithTradeMark { get { return BrandName + "" TM""; } }

	            public String FormatName(String name)
	            {
		            return name.ToUpper();
	            }
            }";
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
            Assert.AreEqual("Brand Name TM", App.Config.BrandNameWithTradeMark);
            Assert.AreEqual("BOB", App.Config.FormatName("bob"));
        }
    }
}
