using Microsoft.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace ZedSharp
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

            throw toThrow ?? new AssertFailedException(typeof(E).Name + " expected");
        }

        /// <summary>Throws exception if code can't compile.</summary>
        public static void Compile(String source, params String[] assemblies)
        {
            var options = new Dictionary<String, String> {{"CompilerVersion", "v4.0"}};
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
        public static void CompileFail(String source, params String[] assemblies)
        {
            Error(() => Compile(source, assemblies), new AssertFailedException("Compile should have failed"));
        }

        /// <summary>Asserts that the actual maybe has a value and the value is equal to the expected value.</summary>
        public static void Some<A>(A expected, Maybe<A> actual)
        {
            Assert.AreEqual(Maybe.Some(expected), actual);
        }

        /// <summary>Asserts that the given maybe has a value.</summary>
        public static void Some<A>(Maybe<A> maybe)
        {
            if (! maybe.HasValue)
                throw new AssertFailedException("Maybe was supposed to have a value");
        }

        /// <summary>Asserts that the given maybe does not have a value.</summary>
        public static void None<A>(Maybe<A> maybe)
        {
            if (maybe.HasValue)
                throw new AssertFailedException("Maybe was not supposed to have a value, but does: " + maybe);
        }
    }
}
