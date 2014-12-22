using System;
using System.IO;

namespace ZedSharp.UnitTests
{
    public static class Common
    {
        public static String ZedDll = Path.GetFileName(typeof(Table).Assembly.CodeBase);

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
