using System;
using System.IO;

namespace KitchenSink.Tests
{
    public static class Common
    {
        public static String ZedDll = new Uri(typeof(Table).Assembly.CodeBase).AbsolutePath;

        public static String Wrap(String source)
        {
            return @"
                using KitchenSink;
                
                namespace XXXXX
                {
                    class YYYYY
                    {
                        static void ZZZZZ()
                        {
                            " + source + @";
                        }
                    }
                }";
        }
    }
}
