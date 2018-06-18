﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using KitchenSink.Extensions;
using static KitchenSink.Operators;

namespace KitchenSink
{
    public class AutoCacheConfig<A>
    {
        internal readonly List<MethodInfo> exclusions = new List<MethodInfo>();
        internal readonly Dictionary<MethodInfo, TimeSpan> expirations = new Dictionary<MethodInfo, TimeSpan>();

        public AutoCacheConfig<A> For(Expression<Action<A>> expr, Action<AutoCacheConfigMethod<A>> doConfig)
        {
            var method = (expr.Body as MethodCallExpression)?.Method
                ?? throw new ArgumentException("Expression must be a method call");
            doConfig(new AutoCacheConfigMethod<A>(this, method));
            return this;
        }
    }

    public class AutoCacheConfigMethod<A>
    {
        private readonly AutoCacheConfig<A> parent;
        private readonly MethodInfo method;

        internal AutoCacheConfigMethod(AutoCacheConfig<A> parent, MethodInfo method)
        {
            this.parent = parent;
            this.method = method;
        }

        public void Exclude() => parent.exclusions.Add(method);

        public void Expire(TimeSpan time) => parent.expirations[method] = time;
    }

    internal static class AutoCache
    {
        private static readonly ConcurrentDictionary<Type, Type> cachedTypes = new ConcurrentDictionary<Type, Type>();

        internal static A Build<A>(A inner, AutoCacheConfig<A> config) where A : class
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

            var generatedType = cachedTypes.GetOrAdd(typeof(A), Build(config));
            return (A) Activator.CreateInstance(generatedType, inner);
        }

        private static Func<Type, Type> Build<A>(AutoCacheConfig<A> config) => interfaceType =>
        {
            var noise = Guid.NewGuid().ToString().Substring(0, 8);
            var assemblyName = new AssemblyName($"{interfaceType.Name}_Assembly_{noise}");
            var assemblyBuilder =
                AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
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
                var arity = paramz.Length;
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

                if (method.ReturnType == typeof(void) || config.exclusions.Contains(method))
                {
                    methodIl.Emit(OpCodes.Ldarg_0);
                    methodIl.Emit(OpCodes.Ldfld, innerFieldBuilder);
                    1.ToIncluding(arity).ForEach(i => methodIl.Emit(OpCodes.Ldarg_S, i));
                    EmitCall(methodIl, method);
                    methodIl.Emit(OpCodes.Nop);
                }
                else if (arity == 0)
                {
                    var cacheType = typeof(Lazy<>).MakeGenericType(method.ReturnType);
                    var cacheFieldBuilder = typeBuilder.DefineField(
                        $"_cache{counter}",
                        cacheType,
                        FieldAttributes.Private);
                    cacheFieldBuilder.SetCustomAttribute(MakeCompilerGeneratedAttribute());

                    ctorIl.Emit(OpCodes.Ldarg_0);
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

                    methodIl.Emit(OpCodes.Ldarg_0);
                    methodIl.Emit(OpCodes.Ldfld, cacheFieldBuilder);
                    var valueMethod = cacheType.GetProperty("Value").NonNull().GetGetMethod();
                    EmitCall(methodIl, valueMethod);
                }
                else if (arity == 1)
                {
                    var keyType = paramz.Single().ParameterType;
                    var cacheType = typeof(ConcurrentDictionary<,>).MakeGenericType(keyType, method.ReturnType);
                    var cacheFieldBuilder = typeBuilder.DefineField(
                        $"_cache{counter}",
                        cacheType,
                        FieldAttributes.Private);
                    cacheFieldBuilder.SetCustomAttribute(MakeCompilerGeneratedAttribute());

                    ctorIl.Emit(OpCodes.Ldarg_0);
                    var dictCtor = cacheType.GetConstructors().Single(x => Empty(x.GetParameters()));
                    ctorIl.Emit(OpCodes.Newobj, dictCtor);
                    ctorIl.Emit(OpCodes.Stfld, cacheFieldBuilder);

                    methodIl.Emit(OpCodes.Ldarg_0);
                    methodIl.Emit(OpCodes.Ldfld, cacheFieldBuilder);
                    methodIl.Emit(OpCodes.Ldarg_1);
                    methodIl.Emit(OpCodes.Ldarg_0);
                    methodIl.Emit(OpCodes.Ldfld, innerFieldBuilder);
                    methodIl.Emit(OpCodes.Dup);
                    methodIl.Emit(OpCodes.Ldvirtftn, method);
                    var funcType = typeof(Func<,>).MakeGenericType(keyType, method.ReturnType);
                    methodIl.Emit(OpCodes.Newobj, funcType.GetConstructors().Single());
                    var getOrAddMethod = cacheType.GetMethods()
                        .Single(x => x.Name == "GetOrAdd"
                             && x.GetParameters().Length == 2
                             && x.GetParameters()[1].ParameterType.Name.Contains("Func"));
                    EmitCall(methodIl, getOrAddMethod);
                }
                else
                {
                    var keyType = Type.GetType($"System.ValueTuple`{arity}")
                        .NonNull()
                        .MakeGenericType(paramz.Select(x => x.ParameterType).ToArray());
                    var cacheType = typeof(ConcurrentDictionary<,>).MakeGenericType(keyType, method.ReturnType);
                    var cacheFieldBuilder = typeBuilder.DefineField(
                        $"_cache{counter}",
                        cacheType,
                        FieldAttributes.Private);
                    cacheFieldBuilder.SetCustomAttribute(MakeCompilerGeneratedAttribute());

                    ctorIl.Emit(OpCodes.Ldarg_0);
                    var dictCtor = cacheType.GetConstructors().Single(x => Empty(x.GetParameters()));
                    ctorIl.Emit(OpCodes.Newobj, dictCtor);
                    ctorIl.Emit(OpCodes.Stfld, cacheFieldBuilder);

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
                    1.ToIncluding(arity).ForEach(i =>
                    {
                        var itemField = keyType.NonNull().GetField($"Item{i}").NonNull();
                        lambdaIl.Emit(OpCodes.Ldarg_1);
                        lambdaIl.Emit(OpCodes.Ldfld, itemField);
                    });
                    EmitCall(lambdaIl, method);
                    lambdaIl.Emit(OpCodes.Ret);

                    methodIl.Emit(OpCodes.Ldarg_0);
                    methodIl.Emit(OpCodes.Ldfld, cacheFieldBuilder);
                    1.ToIncluding(arity).ForEach(i => methodIl.Emit(OpCodes.Ldarg_S, i));
                    methodIl.Emit(OpCodes.Newobj, keyType.NonNull().GetConstructors().Single());
                    methodIl.Emit(OpCodes.Ldarg_0);
                    methodIl.Emit(OpCodes.Ldftn, lambdaBuilder);
                    var funcType = typeof(Func<,>).MakeGenericType(keyType, method.ReturnType);
                    methodIl.Emit(OpCodes.Newobj, funcType.GetConstructors().Single());
                    var getOrAddMethod = cacheType.GetMethods()
                        .Single(x => x.Name == "GetOrAdd"
                             && x.GetParameters().Length == 2
                             && x.GetParameters()[1].ParameterType.Name.Contains("Func"));
                    EmitCall(methodIl, getOrAddMethod);
                }

                methodIl.Emit(OpCodes.Ret);
            }

            ctorIl.Emit(OpCodes.Ret);
            return typeBuilder.CreateType();
        };

        private static CustomAttributeBuilder MakeCompilerGeneratedAttribute() =>
            new CustomAttributeBuilder(
                typeof(CompilerGeneratedAttribute)
                    .GetConstructors().Single(x => Empty(x.GetParameters())),
                ArrayOf<object>());

        private static void EmitCall(ILGenerator il, MethodInfo method) =>
            il.Emit(method.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, method);
    }
}
