using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Microsoft.CSharp;
using KitchenSink.Extensions;
using static KitchenSink.Operators;

namespace KitchenSink
{
    internal static class AutoCache
    {
        private static readonly ConcurrentDictionary<Type, Type> cachedTypes = new ConcurrentDictionary<Type, Type>();

        internal static A Build<A>(A inner) where A : class
        {
            if (Null(inner))
            {
                throw new ArgumentNullException(nameof(inner));
            }

            if (Not(typeof(A).IsInterface))
            {
                throw new ArgumentException($"Given type {typeof(A).FullName} is not an interface type");
            }

            if (NonEmpty(typeof(A).GetProperties()))
            {
                throw new ArgumentException($"Given type {typeof(A).FullName} should not have properties");
            }

            if (typeof(A).IsGenericTypeDefinition)
            {
                throw new ArgumentException($"Given type {typeof(A).FullName} should not be generic");
            }

            var generatedType = cachedTypes.GetOrAdd(typeof(A), Build);
            return (A) Activator.CreateInstance(generatedType, inner);
        }

        private static Type BuildTheCodeDomWay(Type interfaceType)
        {
            var noise = Guid.NewGuid().ToString().Substring(0, 8);
            var type = new CodeTypeDeclaration($"{interfaceType.Name}_Cached_{noise}")
            {
                Attributes = MemberAttributes.Public
            };
            type.BaseTypes.Add(interfaceType);
            var innerField = new CodeMemberField(interfaceType, "_inner")
            {
                Attributes = MemberAttributes.Private
            };
            type.Members.Add(innerField);
            var ctor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };
            var innerParam = new CodeParameterDeclarationExpression(interfaceType, "inner");
            ctor.Parameters.Add(innerParam);
            ctor.Statements.Add(new CodeAssignStatement(
                new CodeFieldReferenceExpression(
                    new CodeThisReferenceExpression(),
                    innerField.Name),
                new CodeArgumentReferenceExpression(innerParam.Name)));
            type.Members.Add(ctor);

            foreach (var (counter, method) in interfaceType.GetMethods().ZipWithIndex())
            {
                var paramz = method.GetParameters();
                var methodDecl = new CodeMemberMethod
                {
                    Name = method.Name,
                    ReturnType = new CodeTypeReference(method.ReturnType),
                };
                methodDecl.Attributes = MemberAttributes.Public;
                methodDecl.Parameters.AddRange(paramz.Select(p =>
                    new CodeParameterDeclarationExpression(p.ParameterType, p.Name)).ToArray());
                type.Members.Add(methodDecl);

                if (method.ReturnType == typeof(void))
                {
                    methodDecl.Statements.Add(
                        new CodeMethodInvokeExpression(
                            new CodeFieldReferenceExpression(
                                new CodeThisReferenceExpression(),
                                innerField.Name),
                            method.Name,
                            methodDecl.Parameters
                                .Cast<CodeParameterDeclarationExpression>()
                                .Select(p => new CodeArgumentReferenceExpression(p.Name))
                                .ToArray()));
                }
                else if (Empty(paramz))
                {
                    var cacheName = $"_cache{counter}";
                    var cacheType = typeof(Lazy<>).MakeGenericType(method.ReturnType);
                    var cacheField = new CodeMemberField(cacheType, cacheName);
                    type.Members.Add(cacheField);
                    ctor.Statements.Add(
                        new CodeAssignStatement(
                            new CodeFieldReferenceExpression(
                                new CodeThisReferenceExpression(),
                                cacheField.Name),
                            new CodeObjectCreateExpression(
                                cacheType,
                                new CodeMethodReferenceExpression(
                                    new CodeFieldReferenceExpression(
                                        new CodeThisReferenceExpression(),
                                        innerField.Name),
                                    method.Name))));
                    methodDecl.Statements.Add(
                        new CodeMethodReturnStatement(
                            new CodePropertyReferenceExpression(
                                new CodeFieldReferenceExpression(
                                    new CodeThisReferenceExpression(),
                                    cacheField.Name),
                                "Value")));
                }
                else if (One(paramz))
                {
                    var cacheName = $"_cache{counter}";
                    var cacheType = typeof(ConcurrentDictionary<,>)
                        .MakeGenericType(paramz.Single().ParameterType, method.ReturnType);
                    var cacheField = new CodeMemberField(cacheType, cacheName);
                    type.Members.Add(cacheField);
                    ctor.Statements.Add(
                        new CodeAssignStatement(
                            new CodeFieldReferenceExpression(
                                new CodeThisReferenceExpression(),
                                cacheField.Name),
                            new CodeObjectCreateExpression(cacheType)));
                    methodDecl.Statements.Add(
                        new CodeMethodReturnStatement(
                            new CodeMethodInvokeExpression(
                                new CodeFieldReferenceExpression(
                                    new CodeThisReferenceExpression(),
                                    cacheField.Name),
                                "GetOrAdd",
                                ArrayOf<CodeExpression>(
                                    new CodeArgumentReferenceExpression(methodDecl.Parameters[0].Name),
                                    new CodeMethodReferenceExpression(
                                        new CodeFieldReferenceExpression(
                                            new CodeThisReferenceExpression(),
                                            innerField.Name),
                                        method.Name)))));
                }
                else
                {
                    var keyType =
                        Type.GetType($"System.ValueTuple`{paramz.Length}").NonNull()
                            .MakeGenericType(paramz.Select(x => x.ParameterType).ToArray());
                    var funcType = typeof(Func<,>).MakeGenericType(keyType, method.ReturnType);
                    var cacheName = $"_cache{counter}";
                    var cacheType = typeof(ConcurrentDictionary<,>).MakeGenericType(keyType, method.ReturnType);
                    var cacheField = new CodeMemberField(cacheType, cacheName);
                    type.Members.Add(cacheField);
                    ctor.Statements.Add(
                        new CodeAssignStatement(
                            new CodeFieldReferenceExpression(
                                new CodeThisReferenceExpression(),
                                cacheField.Name),
                            new CodeObjectCreateExpression(cacheType)));
                    methodDecl.Statements.Add(
                        new CodeMethodReturnStatement(
                            new CodeMethodInvokeExpression(
                                new CodeFieldReferenceExpression(
                                    new CodeThisReferenceExpression(),
                                    cacheField.Name),
                                "GetOrAdd",
                                ArrayOf<CodeExpression>(
                                    new CodeObjectCreateExpression(
                                        new CodeTypeReference(keyType),
                                        paramz.Select(p => new CodeArgumentReferenceExpression(p.Name)).ToArray()),
                                    new CodeObjectCreateExpression(
                                        funcType,
                                        new CodeSnippetExpression($"t => _inner.{method.Name}({1.ToIncluding(paramz.Length).Select(i => $"t.Item{i}").MakeString(",")})"))))));
                }
            }

            var ns = new CodeNamespace($"{interfaceType.Name}_Namespace_{noise}");
            ns.Types.Add(type);
            var compileUnit = new CodeCompileUnit();
            compileUnit.Namespaces.Add(ns);
            var provider = new CSharpCodeProvider();
            var buffer = new StringWriter();
            provider.GenerateCodeFromCompileUnit(compileUnit, buffer, new CodeGeneratorOptions());
            Console.WriteLine(buffer);
            var parameters = new CompilerParameters();
            parameters.ReferencedAssemblies.Add(new Uri(interfaceType.Assembly.CodeBase).LocalPath);
            var results = provider.CompileAssemblyFromDom(parameters, compileUnit);

            if (results.Errors.Count > 0)
            {
                throw new Exception(results.Errors.Cast<CompilerError>().MakeString("\r\n\r\n"));
            }

            return results.CompiledAssembly.GetType(ns.Name + "." + type.Name);
        }

        private static Type Build(Type interfaceType)
        {
            var noise = Guid.NewGuid().ToString().Substring(0, 8);
            var assemblyName = new AssemblyName($"{interfaceType.Name}_Assembly_{noise}");
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            assemblyBuilder.SetCustomAttribute(MakeCompilerGeneratedAttribute());
            var typeName = $"{interfaceType.Name}_Cached_{noise}";
            var moduleBuilder = assemblyBuilder.DefineDynamicModule($"{interfaceType.Name}_Module_{noise}");
            var typeBuilder = moduleBuilder.DefineType(
                typeName,
                TypeAttributes.Public
                | TypeAttributes.AutoClass
                | TypeAttributes.AnsiClass
                | TypeAttributes.Class
                | TypeAttributes.BeforeFieldInit
                | TypeAttributes.AutoLayout,
                null);
            typeBuilder.SetCustomAttribute(MakeCompilerGeneratedAttribute());
            typeBuilder.SetParent(typeof(object));
            typeBuilder.AddInterfaceImplementation(interfaceType);
            var innerFieldBuilder = typeBuilder.DefineField(
                "_inner",
                interfaceType,
                FieldAttributes.Private
                | FieldAttributes.InitOnly);
            innerFieldBuilder.SetCustomAttribute(MakeCompilerGeneratedAttribute());
            var ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public
                | MethodAttributes.HideBySig
                | MethodAttributes.SpecialName
                | MethodAttributes.RTSpecialName,
                CallingConventions.Standard,
                ArrayOf(interfaceType));
            ctorBuilder.SetCustomAttribute(MakeCompilerGeneratedAttribute());
            var ctorIl = ctorBuilder.GetILGenerator();
            ctorIl.Emit(OpCodes.Ldarg_0);
            ctorIl.Emit(OpCodes.Call, typeof(object).GetConstructors().Single());
            ctorIl.Emit(OpCodes.Ldarg_0);
            ctorIl.Emit(OpCodes.Ldarg_1);
            ctorIl.Emit(OpCodes.Stfld, innerFieldBuilder);

            foreach (var (counter, method) in interfaceType.GetMethods().ZipWithIndex())
            {
                var paramz = method.GetParameters();
                var methodBuilder = typeBuilder.DefineMethod(
                    method.Name,
                    MethodAttributes.Public
                    | MethodAttributes.Final
                    | MethodAttributes.HideBySig
                    | MethodAttributes.NewSlot
                    | MethodAttributes.Virtual,
                    CallingConventions.Standard,
                    method.ReturnType,
                    paramz.Select(x => x.ParameterType).ToArray());
                methodBuilder.SetCustomAttribute(MakeCompilerGeneratedAttribute());
                var methodIl = methodBuilder.GetILGenerator();

                if (method.ReturnType == typeof(void))
                {
                    methodIl.Emit(OpCodes.Ldarg_0);
                    methodIl.Emit(OpCodes.Ldfld, innerFieldBuilder);
                    1.ToIncluding(paramz.Length).ForEach(i => methodIl.Emit(OpCodes.Ldarg_S, i));
                    EmitCall(methodIl, method);
                    methodIl.Emit(OpCodes.Nop);
                }
                else
                {
                    var keyType = 
                        paramz.Length > 0 ?
                            paramz.Length > 1
                                ? Type.GetType($"System.ValueTuple`{paramz.Length}")
                                    .NonNull()
                                    .MakeGenericType(paramz.Select(x => x.ParameterType).ToArray())
                                : paramz.Single().ParameterType
                            : null;
                    var cacheType = paramz.Length == 0
                        ? typeof(Lazy<>).MakeGenericType(method.ReturnType)
                        : typeof(ConcurrentDictionary<,>).MakeGenericType(keyType, method.ReturnType);
                    var cacheFieldBuilder = typeBuilder.DefineField(
                        $"_cache{counter}",
                        cacheType,
                        FieldAttributes.Private);
                    cacheFieldBuilder.SetCustomAttribute(MakeCompilerGeneratedAttribute());
                    ctorIl.Emit(OpCodes.Ldarg_0);

                    if (paramz.Length == 0)
                    {
                        ctorIl.Emit(OpCodes.Ldarg_0);
                        ctorIl.Emit(OpCodes.Ldfld, innerFieldBuilder);
                        ctorIl.Emit(OpCodes.Dup);
                        ctorIl.Emit(OpCodes.Ldvirtftn, method);
                        var funcType = typeof(Func<>).MakeGenericType(ArrayOf(method.ReturnType));
                        ctorIl.Emit(OpCodes.Newobj, funcType.GetConstructors().Single());
                        var lazyCtor = cacheType.GetConstructors().Single(x =>
                            x.GetParameters().Length == 1
                            && x.GetParameters().Single().ParameterType.Name.Contains("Func"));
                        ctorIl.Emit(OpCodes.Newobj, lazyCtor);
                        ctorIl.Emit(OpCodes.Stfld, cacheFieldBuilder);
                    }
                    else
                    {
                        var dictCtor = cacheType.GetConstructors().Single(x => Empty(x.GetParameters()));
                        ctorIl.Emit(OpCodes.Newobj, dictCtor);
                        ctorIl.Emit(OpCodes.Stfld, cacheFieldBuilder);
                    }

                    if (paramz.Length == 0)
                    {
                        methodIl.Emit(OpCodes.Ldarg_0);
                        methodIl.Emit(OpCodes.Ldfld, cacheFieldBuilder);
                        var valueMethod = cacheType.GetProperty("Value").NonNull().GetGetMethod();
                        EmitCall(methodIl, valueMethod);
                    }
                    else if (paramz.Length == 1)
                    {
                        methodIl.Emit(OpCodes.Ldarg_0);
                        methodIl.Emit(OpCodes.Ldfld, cacheFieldBuilder);
                        methodIl.Emit(OpCodes.Ldarg_1);
                        methodIl.Emit(OpCodes.Ldarg_0);
                        methodIl.Emit(OpCodes.Ldfld, innerFieldBuilder);
                        methodIl.Emit(OpCodes.Dup);
                        methodIl.Emit(OpCodes.Ldvirtftn, method);
                        var funcType = Type.GetType($"System.Func`{paramz.Length + 1}")
                            .NonNull()
                            .MakeGenericType(
                                paramz.Select(x => x.ParameterType)
                                    .ToArray()
                                    .Concat(method.ReturnType));
                        methodIl.Emit(OpCodes.Newobj, funcType.GetConstructors().Single());
                        var getOrAddMethod = cacheType.GetMethods()
                            .Single(x => x.Name == "GetOrAdd"
                                         && x.GetParameters().Length == 2
                                         && x.GetParameters()[1].ParameterType.Name.Contains("Func"));
                        EmitCall(methodIl, getOrAddMethod);
                    }
                    else
                    {
                        var lambdaBuilder = typeBuilder.DefineMethod(
                            $"<{method.Name}>_{counter}_{noise}",
                            MethodAttributes.Private
                            | MethodAttributes.HideBySig,
                            CallingConventions.Standard,
                            method.ReturnType,
                            ArrayOf(keyType));
                        lambdaBuilder.SetCustomAttribute(MakeCompilerGeneratedAttribute());
                        var lambdaIl = lambdaBuilder.GetILGenerator();
                        lambdaIl.Emit(OpCodes.Ldarg_0);
                        lambdaIl.Emit(OpCodes.Ldfld, innerFieldBuilder);
                        1.ToIncluding(paramz.Length).ForEach(i =>
                        {
                            var itemField = keyType.NonNull().GetField($"Item{i}").NonNull();
                            lambdaIl.Emit(OpCodes.Ldarg_1);
                            lambdaIl.Emit(OpCodes.Ldfld, itemField);
                        });
                        EmitCall(lambdaIl, method);
                        lambdaIl.Emit(OpCodes.Ret);

                        methodIl.Emit(OpCodes.Ldarg_0);
                        methodIl.Emit(OpCodes.Ldfld, cacheFieldBuilder);
                        1.ToIncluding(paramz.Length).ForEach(i => methodIl.Emit(OpCodes.Ldarg_S, i));
                        methodIl.Emit(OpCodes.Newobj, keyType.NonNull().GetConstructors().Single());
                        methodIl.Emit(OpCodes.Ldarg_0);
                        methodIl.Emit(OpCodes.Ldftn, lambdaBuilder);
                        //var funcType = Type.GetType($"System.Func`{paramz.Length + 1}")
                        //    .NonNull()
                        //    .MakeGenericType(
                        //        paramz.Select(x => x.ParameterType)
                        //            .ToArray()
                        //            .Concat(method.ReturnType));
                        var funcType = typeof(Func<,>).MakeGenericType(keyType, method.ReturnType);
                        methodIl.Emit(OpCodes.Newobj, funcType.GetConstructors().Single());
                        var getOrAddMethod = cacheType.GetMethods()
                            .Single(x => x.Name == "GetOrAdd"
                                 && x.GetParameters().Length == 2
                                 && x.GetParameters()[1].ParameterType.Name.Contains("Func"));
                        EmitCall(methodIl, getOrAddMethod);
                    }
                }

                methodIl.Emit(OpCodes.Ret);
            }

            ctorIl.Emit(OpCodes.Ret);
            return typeBuilder.CreateType();
        }

        //public static object MultiParam(object wrapper, object key, int counter, string methodName)
        //{
        //    var wrapperType = wrapper.GetType();
        //    var innerField = wrapperType.GetField("_inner");
        //    var inner = innerField.GetValue(wrapper);
        //    var innerType = inner.GetType();
        //    var cacheField = wrapperType.GetField("_cache" + counter);
        //    var cache = cacheField.GetValue(wrapper);
        //    var cacheType = cache.GetType();
        //    var keyType = cacheType.GetGenericArguments()[0];
        //    var valueType = cacheType.GetGenericArguments()[1];
        //    var funcType = typeof(Func<,>).MakeGenericType(keyType, valueType);
        //    var getOrAdd = cacheType.GetMethod("GetOrAdd", ArrayOf(keyType, funcType));
        //    var keyParam = Expression.Parameter(keyType);
        //    var funcParam = Expression.Parameter(funcType);
        //    var argTypes = keyType.GetGenericArguments();
        //    var innerMethod = innerType.GetMethod(methodName, argTypes);
        //    var body = Expression.Call(innerMethod, ArrayOf(inner).Concat(args)); // TODO: Expression.Call(inner, innerMethod, key.Item1, key.Item2)
        //    var lambda = Expression.Lambda(body, keyParam, funcParam);
        //    var func = lambda.Compile();
        //    return getOrAdd.Invoke(cache, ArrayOf(key, func));
        //}

        private static CustomAttributeBuilder MakeCompilerGeneratedAttribute() =>
            new CustomAttributeBuilder(
                typeof(CompilerGeneratedAttribute)
                    .GetConstructors().Single(x => Empty(x.GetParameters())),
                ArrayOf<object>());

        private static void EmitCall(ILGenerator il, MethodInfo method) =>
            il.Emit(method.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, method);
    }
}
