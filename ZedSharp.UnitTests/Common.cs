using System;
using System.IO;

namespace ZedSharp.UnitTests
{
    public static class Common
    {
        public static String ZedDll = new Uri(typeof(Table).Assembly.CodeBase).AbsolutePath;

        public static String Wrap(String source)
        {
            return @"
                using ZedSharp;
                
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
