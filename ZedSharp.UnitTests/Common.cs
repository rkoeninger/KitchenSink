using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
