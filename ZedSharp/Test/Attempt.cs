using Microsoft.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace ZedSharp.Test
{
    public static class Attempt
    {
        /// <summary>
        /// Catches exception thrown by <code>f</code> and returns it.
        /// Throws exception if none thrown by <code>f</code>.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="toThrow"></param>
        /// <returns></returns>
        public static Exception Catch(Action f, Exception toThrow = null)
        {
            try
            {
                f();
            }
            catch (Exception e)
            {
                return e;
            }

            throw toThrow ?? new AssertFailedException("Exception expected");
        }

        /// <summary>Throws exception if code can't compile.</summary>
        /// <param name="source"></param>
        public static void Compile(String source, params String[] assemblies)
        {
            var options = new Dictionary<String, String>();
            options.Add("CompilerVersion", "v4.0");
            var provider = new CSharpCodeProvider(options);
            var parameters = new CompilerParameters(assemblies);
            parameters.ReferencedAssemblies.Add("mscorlib.dll");
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.GenerateInMemory = true;

            var results = provider.CompileAssemblyFromSource(parameters, source);

            if (results.Errors.HasErrors)
            {
                foreach (var e in results.Errors)
                    Console.WriteLine(e);

                throw new AssertFailedException("Script compiler failed.");
            }
        }

        /// <summary>Throws exception if code compiles (it shouldn't).</summary>
        /// <remarks>This is used to test that type-level programming will invalidate certain uses.</remarks>
        /// <param name="source"></param>
        public static void CompileFail(String source, params String[] assemblies)
        {
            Catch(() => Compile(source, assemblies), new AssertFailedException("Compile should have failed"));
        }
    }
}
