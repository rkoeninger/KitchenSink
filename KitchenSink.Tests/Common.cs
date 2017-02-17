using System;

namespace KitchenSink.Tests
{
    public static class Common
    {
        public static String KsDll = new Uri(typeof(Maybe).Assembly.CodeBase).AbsolutePath;

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
