using Microsoft.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
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

        public static AssertFailedException FailedAssert(Action f)
        {
            return Error<AssertFailedException>(f);
        }

        private static CompilerResults DoCompile(String source, params String[] assemblies)
        {
            var options = new Dictionary<String, String> { { "CompilerVersion", "v4.0" } };
            var provider = new CSharpCodeProvider(options);
            var parameters = new CompilerParameters(assemblies);
            parameters.ReferencedAssemblies.Add("mscorlib.dll");
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.GenerateInMemory = true;
            return provider.CompileAssemblyFromSource(parameters, source);
        }

        /// <summary>
        /// Throws exception if code doesn't fail to compile due to given errors.
        /// </summary>
        public static void CompileFail(String source, String[] assemblies, string[] errorCodes)
        {
            var results = DoCompile(source, assemblies);

            if (results.Errors.HasErrors)
            {
                foreach (var e in results.Errors)
                {
                    Console.WriteLine(e);
                    Console.WriteLine();
                }

                var errorNumbers = results.Errors.GetEnumerator()
                    .AsEnumerable<CompilerError>()
                    .Select(x => x.ErrorNumber)
                    .OrderBy(x => x);

                if (! errorNumbers.SequenceEqual(errorCodes.OrderBy(x => x)))
                {
                    throw new AssertFailedException("Unexpected compiler errors present / Expected compiler errors present - check standard out");
                }
            }
            else
            {
                throw new AssertFailedException("Expected compilation failure");
            }
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
            {
                throw new AssertFailedException("Maybe was supposed to have a value");
            }
        }

        /// <summary>Asserts that the given maybe does not have a value.</summary>
        public static void None<A>(Maybe<A> maybe)
        {
            if (maybe.HasValue)
            {
                throw new AssertFailedException("Maybe was not supposed to have a value, but does: " + maybe);
            }
        }
    }
}
