// TODO: this whole class needs to be re-worked
//       dotnet core does not have Reflection.Emit

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
//using System.Reflection.Emit;
//using System.Runtime.CompilerServices;
using KitchenSink.Extensions;
using static KitchenSink.Operators;

namespace KitchenSink
{
    public class AutoCacheConfig
    {
        internal ISet<MethodInfo> Exclusions { get; } = new HashSet<MethodInfo>();
        internal Dictionary<MethodInfo, TimeSpan> Expirations { get; } = new Dictionary<MethodInfo, TimeSpan>();

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
            Exclusions.Add(expr.GetMethod());
            return this;
        }

        public AutoCacheConfig<A> Expire(TimeSpan time, Expression<Action<A>> expr)
        {
            Expirations[expr.GetMethod()] = time;
            return this;
        }
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

            return inner;
            //var generatedType = cachedTypes.GetOrAdd((config, typeof(A)), Build);
            //return (A) Activator.CreateInstance(generatedType, inner);
        }

        //private static Type Build((AutoCacheConfig, Type) args)
        //{
        //    var (config, interfaceType) = args;
        //    var noise = Guid.NewGuid().ToString().Substring(0, 8);
        //    var assemblyName = new AssemblyName($"{interfaceType.Name}_Assembly_{noise}");
        //    var assemblyBuilder =
        //        AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        //    assemblyBuilder.SetCustomAttribute(MakeCompilerGeneratedAttribute());
        //    var typeName = $"{interfaceType.Name}_Cached_{noise}";
        //    var moduleBuilder = assemblyBuilder.DefineDynamicModule($"{interfaceType.Name}_Module_{noise}");

        //    // public class TypeName : object, InterfaceType { ...
        //    var typeBuilder = moduleBuilder.DefineType(
        //        typeName,
        //        TypeAttributes.Public
        //        | TypeAttributes.AutoClass
        //        | TypeAttributes.AnsiClass
        //        | TypeAttributes.Class
        //        | TypeAttributes.BeforeFieldInit
        //        | TypeAttributes.AutoLayout,
        //        null);
        //    typeBuilder.SetCustomAttribute(MakeCompilerGeneratedAttribute());
        //    typeBuilder.SetParent(typeof(object));
        //    typeBuilder.AddInterfaceImplementation(interfaceType);

        //    // private readonly InterfaceType _inner;
        //    var innerFieldBuilder = typeBuilder.DefineField(
        //        "_inner",
        //        interfaceType,
        //        FieldAttributes.Private
        //        | FieldAttributes.InitOnly);
        //    innerFieldBuilder.SetCustomAttribute(MakeCompilerGeneratedAttribute());

        //    // public TypeName(InterfaceType inner) : base() { this._inner = inner; ...
        //    var ctorBuilder = typeBuilder.DefineConstructor(
        //        MethodAttributes.Public
        //        | MethodAttributes.HideBySig
        //        | MethodAttributes.SpecialName
        //        | MethodAttributes.RTSpecialName,
        //        CallingConventions.Standard,
        //        ArrayOf(interfaceType));
        //    ctorBuilder.SetCustomAttribute(MakeCompilerGeneratedAttribute());
        //    var ctorIl = ctorBuilder.GetILGenerator();
        //    ctorIl.Emit(OpCodes.Ldarg_0);
        //    ctorIl.Emit(OpCodes.Call, typeof(object).GetConstructors().Single());
        //    ctorIl.Emit(OpCodes.Ldarg_0);
        //    ctorIl.Emit(OpCodes.Ldarg_1);
        //    ctorIl.Emit(OpCodes.Stfld, innerFieldBuilder);

        //    foreach (var (counter, method) in interfaceType.GetMethods().ZipWithIndex())
        //    {
        //        var hasExpiration = config.Expirations.TryGetValue(method, out var timeout);
        //        var paramz = method.GetParameters();
        //        var parameterTypes = paramz.Select(x => x.ParameterType).ToArray();

        //        // public TReturn Method(TParam0 param0, TParam1 param1, ...) { ...
        //        var methodBuilder = typeBuilder.DefineMethod(
        //            method.Name,
        //            MethodAttributes.Public
        //            | MethodAttributes.Final
        //            | MethodAttributes.HideBySig
        //            | MethodAttributes.NewSlot
        //            | MethodAttributes.Virtual,
        //            CallingConventions.Standard,
        //            method.ReturnType,
        //            paramz.Select(x => x.ParameterType).ToArray());
        //        methodBuilder.SetCustomAttribute(MakeCompilerGeneratedAttribute());
        //        var methodIl = methodBuilder.GetILGenerator();

        //        if (method.ReturnType == typeof(void) || config.Exclusions.Contains(method))
        //        {
        //            // ?return _inner.Method(args...)
        //            methodIl.Emit(OpCodes.Ldarg_0);
        //            methodIl.Emit(OpCodes.Ldfld, innerFieldBuilder);
        //            1.ToIncluding(paramz.Length).ForEach(i => methodIl.Emit(OpCodes.Ldarg_S, i));
        //            EmitCall(methodIl, method);
        //            methodIl.Emit(OpCodes.Nop);
        //            methodIl.Emit(OpCodes.Ret);
        //        }
        //        else
        //        {
        //            var keyType = KeyType(parameterTypes);
        //            var valueType = method.ReturnType;
        //            var cacheType = typeof(Func<,>).MakeGenericType(keyType, valueType);

        //            // private readonly Func<TKey, TReturn> _cacheN;
        //            var cacheFieldBuilder = typeBuilder.DefineField(
        //                $"_cache{counter}",
        //                cacheType,
        //                FieldAttributes.Private
        //                | FieldAttributes.InitOnly);
        //            cacheFieldBuilder.SetCustomAttribute(MakeCompilerGeneratedAttribute());

        //            // private TReturn Lambda(TKey key) =>
        //            //     this._inner.Method();
        //            //  OR this._inner.Method(key);
        //            //  OR this._inner.Method(key.Item1, key.Item2, ...);
        //            var lambdaBuilder = typeBuilder.DefineMethod(
        //                $"<{method.Name}>_{counter}_{noise}",
        //                MethodAttributes.Private
        //                | MethodAttributes.HideBySig,
        //                CallingConventions.Standard,
        //                valueType,
        //                ArrayOf(keyType));
        //            lambdaBuilder.SetCustomAttribute(MakeCompilerGeneratedAttribute());
        //            var lambdaIl = lambdaBuilder.GetILGenerator();
        //            lambdaIl.Emit(OpCodes.Ldarg_0);
        //            lambdaIl.Emit(OpCodes.Ldfld, innerFieldBuilder);
        //            EmitLoadInnerMethodArgs(lambdaIl, keyType, paramz.Length);
        //            EmitCall(lambdaIl, method);
        //            lambdaIl.Emit(OpCodes.Ret);

        //            // this._cacheN = Memo<TKey, TReturn>(?timespan, this.Lambda)
        //            ctorIl.Emit(OpCodes.Ldarg_0);
        //            EmitTimeout(ctorIl, hasExpiration, timeout);
        //            ctorIl.Emit(OpCodes.Ldarg_0);
        //            ctorIl.Emit(OpCodes.Ldftn, lambdaBuilder);
        //            var funcType = typeof(Func<,>).MakeGenericType(keyType, valueType);
        //            ctorIl.Emit(OpCodes.Newobj, funcType.GetConstructors().Single());
        //            EmitCall(ctorIl, MemoMethod(hasExpiration, keyType, valueType));
        //            ctorIl.Emit(OpCodes.Stfld, cacheFieldBuilder);

        //            // this._cacheN.Invoke(~key)
        //            methodIl.Emit(OpCodes.Ldarg_0);
        //            methodIl.Emit(OpCodes.Ldfld, cacheFieldBuilder);
        //            EmitLoadCacheMethodArgs(methodIl, keyType, paramz.Length);
        //            var getMethod = cacheType.GetMethod("Invoke").NonNull();
        //            EmitCall(methodIl, getMethod);
        //            methodIl.Emit(OpCodes.Ret);
        //        }
        //    }

        //    ctorIl.Emit(OpCodes.Ret);
        //    return typeBuilder.CreateType();
        //}

        //private static CustomAttributeBuilder MakeCompilerGeneratedAttribute() =>
        //    new CustomAttributeBuilder(
        //        typeof(CompilerGeneratedAttribute)
        //            .GetConstructors().Single(x => Empty(x.GetParameters())),
        //        ArrayOf<object>());

        //private static Type KeyType(IReadOnlyCollection<Type> parameterTypes)
        //{
        //    switch (parameterTypes.Count)
        //    {
        //        case 0:
        //            return typeof(int);
        //        case 1:
        //            return parameterTypes.Single();
        //        default:
        //            return Type.GetType($"System.ValueTuple`{parameterTypes.Count}").NonNull()
        //                .MakeGenericType(parameterTypes.ToArray());
        //    }
        //}

        //private static MethodInfo MemoMethod(bool hasExpiration, Type keyType, Type valueType) =>
        //    typeof(Operators).GetMethods()
        //        .Single(x =>
        //            x.Name == nameof(Memo)
        //            && x.GetParameters().Length == (hasExpiration ? 2 : 1)
        //            && x.GetParameters()[hasExpiration ? 1 : 0].ParameterType.GenericTypeArguments.Length == 2)
        //        .MakeGenericMethod(ArrayOf(keyType, valueType));

        //private static void EmitLoadInnerMethodArgs(ILGenerator il, Type keyType, int arity)
        //{
        //    switch (arity)
        //    {
        //        case 1:
        //            il.Emit(OpCodes.Ldarg_1);
        //            return;
        //        default:
        //            1.ToIncluding(arity).ForEach(i =>
        //            {
        //                var itemField = keyType.GetField($"Item{i}").NonNull();
        //                il.Emit(OpCodes.Ldarg_1);
        //                il.Emit(OpCodes.Ldfld, itemField);
        //            });
        //            return;
        //    }
        //}

        //private static void EmitLoadCacheMethodArgs(ILGenerator il, Type keyType, int arity)
        //{
        //    switch (arity)
        //    {
        //        case 0:
        //            il.Emit(OpCodes.Ldc_I4_0);
        //            return;
        //        case 1:
        //            il.Emit(OpCodes.Ldarg_1);
        //            return;
        //        default:
        //            1.ToIncluding(arity).ForEach(i => il.Emit(OpCodes.Ldarg_S, i));
        //            il.Emit(OpCodes.Newobj, keyType.GetConstructors().Single());
        //            return;
        //    }
        //}

        //private static void EmitTimeout(ILGenerator il, bool hasExpiration, TimeSpan timeout)
        //{
        //    if (hasExpiration)
        //    {
        //        // new TimeSpan(...)
        //        il.Emit(OpCodes.Ldc_I8, timeout.Ticks);
        //        var timeSpanCtor = typeof(TimeSpan).GetConstructor(ArrayOf(typeof(long))).NonNull();
        //        il.Emit(OpCodes.Newobj, timeSpanCtor);
        //    }
        //}

        //private static void EmitCall(ILGenerator il, MethodInfo method) =>
        //    il.Emit(method.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, method);
    }
}
