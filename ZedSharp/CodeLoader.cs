﻿using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace ZedSharp
{
    public class CodeLoader<T>
    {
        public CodeLoader(String path)
        {
            Path = path;
            LazyLoader = new Lazy<T>(Compile);
            StandardAssemblies = new [] {
                "mscorlib.dll",
                "System.dll",
                "System.Core.dll"
            };
        }

        private readonly String Path;
        private readonly Lazy<T> LazyLoader;
        private readonly String[] StandardAssemblies;
        public T Value { get { return LazyLoader.Value; } }

        public void Load()
        {
            // ReSharper disable UnusedVariable
            var x = LazyLoader.Value;
            // ReSharper restore UnusedVariable
        }

        private T Compile()
        {
            var referencingAssembly = typeof(T).Assembly;
            var options = new Dictionary<String, String> {{"CompilerVersion", "v4.0"}};
            var provider = new CSharpCodeProvider(options);
            var parameters = new CompilerParameters(StandardAssemblies.Add(referencingAssembly.CodeBase.Replace("file:///", "")))
            {
                GenerateInMemory = true,
                IncludeDebugInformation = true
            };
            var results = provider.CompileAssemblyFromFile(parameters, new [] {Path});

            if (results.Errors.HasErrors)
                throw new Exception("");

            var type = results.CompiledAssembly.GetTypes().FirstOrDefault(x => typeof(T).IsAssignableFrom(x));

            if (type == null)
                throw new Exception("");

            var constructor = type.GetConstructor(new Type[0]);

            if (constructor == null)
                throw new Exception("");

            var configObj = constructor.Invoke(new Object[0]);

            if (configObj == null)
                throw new Exception("");

            return (T) configObj;
        }
    }
}
