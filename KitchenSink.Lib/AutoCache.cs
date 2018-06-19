using System;
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
    public class AutoCacheConfig
    {
        public ISet<MethodInfo> Exclusions { get; } = new HashSet<MethodInfo>();
        public Dictionary<MethodInfo, TimeSpan> Expirations { get; } = new Dictionary<MethodInfo, TimeSpan>();

        public override bool Equals(object obj) =>
            obj is AutoCacheConfig that
            && Exclusions.SetEquals(that.Exclusions)
            && Expirations.OrderBy(x => x.Key).SequenceEqual(that.Expirations.OrderBy(x => x.Key));

        public override int GetHashCode() =>
            Exclusions.Aggregate(1, (h, m) => h + 31 * m.GetHashCode())
            + Expirations.OrderBy(x => x.Key).Aggregate(7, (h, m) => h + 37 * m.GetHashCode());
    }

    public class AutoCacheConfig<A> : AutoCacheConfig
    {
        public AutoCacheConfig<A> Exclude(Expression<Action<A>> expr)
        {
            Exclusions.Add(GetMethod(expr));
            return this;
        }

        public AutoCacheConfig<A> Expire(TimeSpan time, Expression<Action<A>> expr)
        {
            Expirations[GetMethod(expr)] = time;
            return this;
        }

        private static MethodInfo GetMethod(Expression<Action<A>> expr) =>
            (expr.Body as MethodCallExpression)?.Method
                ?? throw new ArgumentException("Expression must be a method call");
    }

    internal static class AutoCache
    {
        private static readonly ConcurrentDictionary<(AutoCacheConfig, Type), Type> cachedTypes =
            new ConcurrentDictionary<(AutoCacheConfig, Type), Type>();

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

            var generatedType = cachedTypes.GetOrAdd((config, typeof(A)), Build);
            return (A) Activator.CreateInstance(generatedType, inner);
        }

        private static Type Build((AutoCacheConfig, Type) args)
        {
            var (config, interfaceType) = args;
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
                var hasExpiration = config.Expirations.TryGetValue(method, out var duration);
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

                if (method.ReturnType == typeof(void) || config.Exclusions.Contains(method))
                {
                    methodIl.Emit(OpCodes.Ldarg_0);
                    methodIl.Emit(OpCodes.Ldfld, innerFieldBuilder);
                    1.ToIncluding(arity).ForEach(i => methodIl.Emit(OpCodes.Ldarg_S, i));
                    EmitCall(methodIl, method);
                    methodIl.Emit(OpCodes.Nop);
                    methodIl.Emit(OpCodes.Ret);
                }
                else if (arity == 0)
                {
                    // Field:
                    // private readonly GenericCache<int, string> _cache0;
                    var keyType = typeof(int);
                    var valueType = method.ReturnType;
                    var cacheType = typeof(GenericCache<,>).MakeGenericType(keyType, valueType);
                    var cacheFieldBuilder = typeBuilder.DefineField(
                        $"_cache{counter}",
                        cacheType,
                        FieldAttributes.Private
                        | FieldAttributes.InitOnly);
                    cacheFieldBuilder.SetCustomAttribute(MakeCompilerGeneratedAttribute());

                    // Ctor:
                    // this._cache0 = new GenericCache<int, string>(new TimeSpan(X), _ => this._inner.Get());
                    var lambdaBuilder = typeBuilder.DefineMethod(
                        $"<{method.Name}>_{counter}_{noise}",
                        MethodAttributes.Private
                        | MethodAttributes.HideBySig,
                        CallingConventions.Standard,
                        valueType,
                        ArrayOf(keyType));
                    lambdaBuilder.SetCustomAttribute(MakeCompilerGeneratedAttribute());
                    var lambdaIl = lambdaBuilder.GetILGenerator();
                    lambdaIl.Emit(OpCodes.Ldarg_0);
                    lambdaIl.Emit(OpCodes.Ldfld, innerFieldBuilder);
                    EmitCall(lambdaIl, method);
                    lambdaIl.Emit(OpCodes.Ret);

                    ctorIl.Emit(OpCodes.Ldarg_0);

                    if (hasExpiration)
                    {
                        ctorIl.Emit(OpCodes.Ldc_I8, duration.Ticks);
                        var timeSpanCtor = typeof(TimeSpan).GetConstructor(ArrayOf(typeof(long))).NonNull();
                        ctorIl.Emit(OpCodes.Newobj, timeSpanCtor);
                    }

                    ctorIl.Emit(OpCodes.Ldarg_0);
                    ctorIl.Emit(OpCodes.Ldftn, lambdaBuilder);
                    var funcType = typeof(Func<,>).MakeGenericType(keyType, valueType);
                    ctorIl.Emit(OpCodes.Newobj, funcType.GetConstructors().Single());
                    var cacheCtor = cacheType.GetConstructors()
                        .Single(x => x.GetParameters().Length == (hasExpiration ? 2 : 1));
                    ctorIl.Emit(OpCodes.Newobj, cacheCtor);
                    ctorIl.Emit(OpCodes.Stfld, cacheFieldBuilder);

                    // Method:
                    // public string Get() => this._cache0.Get(0);
                    methodIl.Emit(OpCodes.Ldarg_0);
                    methodIl.Emit(OpCodes.Ldfld, cacheFieldBuilder);
                    methodIl.Emit(OpCodes.Ldc_I4_0);
                    var getMethod = cacheType.GetMethod("Get").NonNull();
                    EmitCall(methodIl, getMethod);
                    methodIl.Emit(OpCodes.Ret);
                }
                else if (arity == 1)
                {
                    // Field:
                    // private readonly GenericCache<int, string> _cache0;
                    var keyType = method.GetParameters().Single().ParameterType;
                    var valueType = method.ReturnType;
                    var cacheType = typeof(GenericCache<,>).MakeGenericType(keyType, valueType);
                    var cacheFieldBuilder = typeBuilder.DefineField(
                        $"_cache{counter}",
                        cacheType,
                        FieldAttributes.Private
                        | FieldAttributes.InitOnly);
                    cacheFieldBuilder.SetCustomAttribute(MakeCompilerGeneratedAttribute());

                    // Ctor:
                    // this._cache0 = new GenericCache<int, string>((int k) => this._inner.Get(k));
                    var lambdaBuilder = typeBuilder.DefineMethod(
                        $"<{method.Name}>_{counter}_{noise}",
                        MethodAttributes.Private
                        | MethodAttributes.HideBySig,
                        CallingConventions.Standard,
                        valueType,
                        ArrayOf(keyType));
                    lambdaBuilder.SetCustomAttribute(MakeCompilerGeneratedAttribute());
                    var lambdaIl = lambdaBuilder.GetILGenerator();
                    lambdaIl.Emit(OpCodes.Ldarg_0);
                    lambdaIl.Emit(OpCodes.Ldfld, innerFieldBuilder);
                    lambdaIl.Emit(OpCodes.Ldarg_1);
                    EmitCall(lambdaIl, method);
                    lambdaIl.Emit(OpCodes.Ret);

                    ctorIl.Emit(OpCodes.Ldarg_0);

                    if (hasExpiration)
                    {
                        ctorIl.Emit(OpCodes.Ldc_I8, duration.Ticks);
                        var timeSpanCtor = typeof(TimeSpan).GetConstructor(ArrayOf(typeof(long))).NonNull();
                        ctorIl.Emit(OpCodes.Newobj, timeSpanCtor);
                    }

                    ctorIl.Emit(OpCodes.Ldarg_0);
                    ctorIl.Emit(OpCodes.Ldftn, lambdaBuilder);
                    var funcType = typeof(Func<,>).MakeGenericType(keyType, valueType);
                    ctorIl.Emit(OpCodes.Newobj, funcType.GetConstructors().Single());
                    var cacheCtor = cacheType.GetConstructors()
                        .Single(x => x.GetParameters().Length == (hasExpiration ? 2 : 1));
                    ctorIl.Emit(OpCodes.Newobj, cacheCtor);
                    ctorIl.Emit(OpCodes.Stfld, cacheFieldBuilder);

                    // Method:
                    // public string Get(int id) => this._cache0.Get(id);
                    methodIl.Emit(OpCodes.Ldarg_0);
                    methodIl.Emit(OpCodes.Ldfld, cacheFieldBuilder);
                    methodIl.Emit(OpCodes.Ldarg_1);
                    var getMethod = cacheType.GetMethod("Get").NonNull();
                    EmitCall(methodIl, getMethod);
                    methodIl.Emit(OpCodes.Ret);
                }
                else
                {
                    // Field:
                    // private readonly GenericCache<(string, char), int> _cache1;
                    var keyType = Type.GetType($"System.ValueTuple`{arity}").NonNull()
                        .MakeGenericType(paramz.Select(x => x.ParameterType).ToArray());
                    var valueType = method.ReturnType;
                    var cacheType = typeof(GenericCache<,>).MakeGenericType(keyType, valueType);
                    var cacheFieldBuilder = typeBuilder.DefineField(
                        $"_cache{counter}",
                        cacheType,
                        FieldAttributes.Private
                        | FieldAttributes.InitOnly);
                    cacheFieldBuilder.SetCustomAttribute(MakeCompilerGeneratedAttribute());

                    // Ctor:
                    // this._cache1 = new GenericCache<(string, char), int>(new TimeSpan(X), k => this._inner.Get(k.Item1, k.Item2));
                    var lambdaBuilder = typeBuilder.DefineMethod(
                        $"<{method.Name}>_{counter}_{noise}",
                        MethodAttributes.Private
                        | MethodAttributes.HideBySig,
                        CallingConventions.Standard,
                        valueType,
                        ArrayOf(keyType));
                    lambdaBuilder.SetCustomAttribute(MakeCompilerGeneratedAttribute());
                    var lambdaIl = lambdaBuilder.GetILGenerator();
                    lambdaIl.Emit(OpCodes.Ldarg_0);
                    lambdaIl.Emit(OpCodes.Ldfld, innerFieldBuilder);
                    1.ToIncluding(arity).ForEach(i =>
                    {
                        var itemField = keyType.GetField($"Item{i}").NonNull();
                        lambdaIl.Emit(OpCodes.Ldarg_1);
                        lambdaIl.Emit(OpCodes.Ldfld, itemField);
                    });
                    EmitCall(lambdaIl, method);
                    lambdaIl.Emit(OpCodes.Ret);

                    ctorIl.Emit(OpCodes.Ldarg_0);

                    if (hasExpiration)
                    {
                        ctorIl.Emit(OpCodes.Ldc_I8, duration.Ticks);
                        var timeSpanCtor = typeof(TimeSpan).GetConstructor(ArrayOf(typeof(long))).NonNull();
                        ctorIl.Emit(OpCodes.Newobj, timeSpanCtor);
                    }

                    ctorIl.Emit(OpCodes.Ldarg_0);
                    ctorIl.Emit(OpCodes.Ldftn, lambdaBuilder);
                    var funcType = typeof(Func<,>).MakeGenericType(keyType, valueType);
                    ctorIl.Emit(OpCodes.Newobj, funcType.GetConstructors().Single());
                    var cacheCtor = cacheType.GetConstructors()
                        .Single(x => x.GetParameters().Length == (hasExpiration ? 2 : 1));
                    ctorIl.Emit(OpCodes.Newobj, cacheCtor);
                    ctorIl.Emit(OpCodes.Stfld, cacheFieldBuilder);

                    // Method:
                    // public string Get(string x, char y) => this._cache0.Get((x, y));
                    methodIl.Emit(OpCodes.Ldarg_0);
                    methodIl.Emit(OpCodes.Ldfld, cacheFieldBuilder);
                    1.ToIncluding(arity).ForEach(i => methodIl.Emit(OpCodes.Ldarg_S, i));
                    methodIl.Emit(OpCodes.Newobj, keyType.GetConstructors().Single());
                    var getMethod = cacheType.GetMethod("Get").NonNull();
                    EmitCall(methodIl, getMethod);
                    methodIl.Emit(OpCodes.Ret);
                }
            }

            ctorIl.Emit(OpCodes.Ret);
            return typeBuilder.CreateType();
        }

        private static CustomAttributeBuilder MakeCompilerGeneratedAttribute() =>
            new CustomAttributeBuilder(
                typeof(CompilerGeneratedAttribute)
                    .GetConstructors().Single(x => Empty(x.GetParameters())),
                ArrayOf<object>());

        private static void EmitCall(ILGenerator il, MethodInfo method) =>
            il.Emit(method.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, method);
    }
}
