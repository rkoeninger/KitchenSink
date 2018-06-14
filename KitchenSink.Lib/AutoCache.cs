using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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

            var generatedType = cachedTypes.GetOrAdd(typeof(A), Build);
            var instance = generatedType.GetConstructors().Single().Invoke(ArrayOf((object) inner));
            return (A) instance;
        }

        private static Type Build(Type interfaceType)
        {
            var noise = Guid.NewGuid().ToString().Substring(0, 8);
            var assemblyName = new AssemblyName($"{interfaceType.Name}_Assembly_{noise}");
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
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
            typeBuilder.SetParent(typeof(object));
            typeBuilder.AddInterfaceImplementation(interfaceType);
            var innerFieldBuilder = typeBuilder.DefineField(
                "_inner",
                interfaceType,
                FieldAttributes.Private
                | FieldAttributes.InitOnly);
            var ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public
                | MethodAttributes.HideBySig
                | MethodAttributes.SpecialName
                | MethodAttributes.RTSpecialName,
                CallingConventions.Standard,
                ArrayOf(interfaceType));
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

                foreach (var (i, param) in paramz.ZipWithIndex())
                {
                    var paramBuilder = methodBuilder.DefineParameter(i, param.Attributes, param.Name);

                    if (param.HasDefaultValue)
                    {
                        paramBuilder.SetConstant(param.DefaultValue);
                    }
                }

                var methodIl = methodBuilder.GetILGenerator();
                methodIl.Emit(OpCodes.Ldarg_0);

                if (method.ReturnType == typeof(void))
                {
                    methodIl.Emit(OpCodes.Ldfld, innerFieldBuilder);
                    1.ToIncluding(paramz.Length).ForEach(i => methodIl.Emit(OpCodes.Ldarg_S, i));
                    methodIl.Emit(OpCodes.Call, method);
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
                    }
                    else
                    {
                        var dictCtor = cacheType.GetConstructors().Single(x => Empty(x.GetParameters()));
                        ctorIl.Emit(OpCodes.Newobj, dictCtor);
                    }

                    ctorIl.Emit(OpCodes.Stfld, cacheFieldBuilder);
                    methodIl.Emit(OpCodes.Ldfld, cacheFieldBuilder);

                    if (paramz.Length == 0)
                    {
                        var valueMethod = cacheType.GetProperty("Value").NonNull().GetGetMethod();
                        methodIl.Emit(OpCodes.Call, valueMethod);
                    }
                    else
                    {
                        if (paramz.Length == 1)
                        {
                            methodIl.Emit(OpCodes.Ldarg_1);
                        }
                        else
                        {
                            1.ToIncluding(paramz.Length).ForEach(i => methodIl.Emit(OpCodes.Ldarg_S, i));
                            methodIl.Emit(OpCodes.Newobj, keyType.NonNull().GetConstructors().Single());

                            // TODO: generate separate lambda and refer when making Func ~10 lines below
                            /*
    .method private hidebysig 
		instance string '<Get2>b__8_0' (
			valuetype [mscorlib]System.ValueTuple`2<int32, string> t
		) cil managed 
	{
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x4fc2
		// Code size 24 (0x18)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld class KitchenSink.Tests.Caching/IUserRepostiory KitchenSink.Tests.Caching/CachedUserRepository::_inner
		IL_0006: ldarg.1
		IL_0007: ldfld !0 valuetype [mscorlib]System.ValueTuple`2<int32, string>::Item1
		IL_000c: ldarg.1
		IL_000d: ldfld !1 valuetype [mscorlib]System.ValueTuple`2<int32, string>::Item2
		IL_0012: callvirt instance string KitchenSink.Tests.Caching/IUserRepostiory::Get2(int32, string)
		IL_0017: ret
	} // end of method CachedUserRepository::'<Get2>b__8_0'
                             */
                        }

                        // TODO: this section is very different for multi-arg methods
                        /*
	.method public final hidebysig newslot virtual 
		instance string Get2 (
			int32 id,
			string x
		) cil managed 
	{
		// Method begins at RVA 0x4f86
		// Code size 31 (0x1f)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: ldfld class [mscorlib]System.Collections.Concurrent.ConcurrentDictionary`2<valuetype [mscorlib]System.ValueTuple`2<int32, string>, string> KitchenSink.Tests.Caching/CachedUserRepository::_cache1
		IL_0006: ldarg.1
		IL_0007: ldarg.2
		IL_0008: newobj instance void valuetype [mscorlib]System.ValueTuple`2<int32, string>::.ctor(!0, !1)
		IL_000d: ldarg.0
		IL_000e: ldftn instance string KitchenSink.Tests.Caching/CachedUserRepository::'<Get2>b__8_0'(valuetype [mscorlib]System.ValueTuple`2<int32, string>)
		IL_0014: newobj instance void class [mscorlib]System.Func`2<valuetype [mscorlib]System.ValueTuple`2<int32, string>, string>::.ctor(object, native int)
		IL_0019: callvirt instance !1 class [mscorlib]System.Collections.Concurrent.ConcurrentDictionary`2<valuetype [mscorlib]System.ValueTuple`2<int32, string>, string>::GetOrAdd(!0, class [mscorlib]System.Func`2<!0, !1>)
		IL_001e: ret
	} // end of method CachedUserRepository::Get2
                         */

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
                        methodIl.Emit(OpCodes.Call, getOrAddMethod);
                    }
                }

                methodIl.Emit(OpCodes.Ret);
            }

            ctorIl.Emit(OpCodes.Ret);
            return typeBuilder.CreateType();
        }
    }
}
