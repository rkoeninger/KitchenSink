using System;

namespace KitchenSink.Tests
{
    public static class Common
    {
        public static string KsDll = new Uri(typeof(Maybe).Assembly.CodeBase).AbsolutePath;

        public static string Wrap(string source)
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
