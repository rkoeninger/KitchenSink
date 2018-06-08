using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KitchenSink.Extensions;
using static KitchenSink.Operators;

namespace KitchenSink
{
    public static class AutoCache
    {
        /// <summary>
        /// Raises error if interface has properties,
        /// void methods will just pass-through.
        /// </summary>
        public static A Build<A>(A inner) where A : class
        {
            if (!typeof(A).IsInterface)
            {
                throw new ArgumentException($"Given type {typeof(A).FullName} is not an interface type");
            }

            if (typeof(A).GetProperties().Length > 0)
            {
                throw new ArgumentException($"Given type {typeof(A).FullName} should not have properties");
            }

            var interfaceType = typeof(A);
            var noise = Guid.NewGuid().ToString().Substring(0, 8);
            var assemblyName = new AssemblyName($"{interfaceType.Name}_Assembly_{noise}");
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var typeName = $"{interfaceType.Name}_Cached_{noise}";
            var moduleBuilder = assemblyBuilder.DefineDynamicModule($"{interfaceType.Name}_Module_{noise}");
            var typeBuilder = moduleBuilder.DefineType(
                typeName,
                TypeAttributes.Public
                | TypeAttributes.Class
                | TypeAttributes.AutoClass
                | TypeAttributes.AnsiClass
                | TypeAttributes.BeforeFieldInit
                | TypeAttributes.AutoLayout,
                null);
            typeBuilder.AddInterfaceImplementation(interfaceType);
            var innerFieldBuilder = typeBuilder.DefineField("_inner", interfaceType, FieldAttributes.Private);
            var ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                ArrayOf(interfaceType));
            var ctorIl = ctorBuilder.GetILGenerator();
            ctorIl.Emit(OpCodes.Ldarg_0);
            ctorIl.Emit(OpCodes.Stfld, innerFieldBuilder);
            ctorIl.Emit(OpCodes.Ret);

            foreach (var (counter, method) in interfaceType.GetMethods().ZipWithIndex())
            {
                var methodBuilder = typeBuilder.DefineMethod(
                    method.Name,
                    method.Attributes,
                    CallingConventions.Standard);
                methodBuilder.SetReturnType(method.ReturnType);
                var methodIl = methodBuilder.GetILGenerator();
                var paramz = method.GetParameters();

                if (method.ReturnType == typeof(void))
                {
                    foreach (var argIndex in Enumerable.Range(1, paramz.Length))
                    {
                        methodIl.Emit(OpCodes.Ldc_I4_S, argIndex);
                        methodIl.Emit(OpCodes.Ldarg_S);
                    }

                    // TODO: call inner method
                }
                else
                {
                    var keyType = method.GetParameters().Length > 1
                        ? Type.GetType($"System.ValueType`{paramz.Length}")
                            .NonNull()
                            .MakeGenericType(paramz.Select(x => x.ParameterType).ToArray())
                        : paramz.Single().ParameterType;
                    var cacheType = paramz.Length == 0
                        ? typeof(Lazy<>).MakeGenericType(method.ReturnType)
                        : typeof(ConcurrentDictionary<,>).MakeGenericType(keyType, method.ReturnType);
                    var cacheFieldBuilder = typeBuilder.DefineField(
                        $"_cache{counter}",
                        cacheType,
                        FieldAttributes.Private);
                    methodIl.Emit(OpCodes.Ldfld, cacheFieldBuilder);

                    if (paramz.Length == 0)
                    {
                        var valueMethod = cacheType.GetProperty("Value").NonNull().GetGetMethod();
                        // TODO: methodIl.EmitCall(OpCode, valueMethod, ArrayOf<Type>());
                    }
                    else
                    {
                        if (paramz.Length == 1)
                        {
                            methodIl.Emit(OpCodes.Ldarg_1);
                        }
                        else if (paramz.Length > 1)
                        {
                            foreach (var argIndex in Enumerable.Range(1, paramz.Length))
                            {
                                methodIl.Emit(OpCodes.Ldc_I4_S, argIndex);
                                methodIl.Emit(OpCodes.Ldarg_S);
                            }

                            methodIl.Emit(OpCodes.Newobj, keyType.GetConstructors().Single());
                        }

                        // TODO: load ref to same method on inner
                        var getOrAddMethod = cacheType.GetMethod("GetOrAdd").NonNull();
                        // TODO: methodIl.EmitCall(OpCode, getOrAddMethod, ArrayOf<Type>());
                    }
                }

                methodIl.Emit(OpCodes.Ret);
            }

            var generatedType = typeBuilder.CreateType();
            var instance = generatedType.GetConstructors().Single().Invoke(ArrayOf((object) inner));
            return (A) instance;
        }
    }
}
