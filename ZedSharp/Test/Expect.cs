using Microsoft.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace ZedSharp.Test
{
    /// <summary>
    /// Expect contains methods that are used to assert failures,
    /// like exception throwing and failed compiliation/type-checking.
    /// 
    /// It is a companion to Assert.
    /// </summary>
    public static class Expect
    {
        /// <summary>
        /// Catches exception thrown by <code>f</code> and returns it.
        /// Throws exception if none thrown by <code>f</code>.
        /// </summary>
        public static Exception Error(Action f, Exception toThrow = null)
        {
            return Error<Exception>(f, toThrow);
        }

        /// <summary>
        /// Catches exception thrown by <code>f</code> and returns it.
        /// Throws exception if none thrown by <code>f</code>.
        /// </summary>
        public static E Error<E>(Action f, Exception toThrow = null) where E : Exception
        {
            try
            {
                f();
            }
            catch (E e)
            {
                return e;
            }

            throw toThrow ?? new AssertFailedException("Exception expected");
        }

        /// <summary>Throws exception if code can't compile.</summary>
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
            Error(() => Compile(source, assemblies), new AssertFailedException("Compile should have failed"));
        }
    }
}
