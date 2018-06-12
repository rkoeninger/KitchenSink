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
        internal static A Build<A>(A inner) where A : class
        {
            if (Not(typeof(A).IsInterface))
            {
                throw new ArgumentException($"Given type {typeof(A).FullName} is not an interface type");
            }

            if (NonEmpty(typeof(A).GetProperties()))
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
                var paramz = method.GetParameters();
                var methodBuilder = typeBuilder.DefineMethod(
                    method.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual,
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

                if (method.ReturnType == typeof(void))
                {
                    methodIl.Emit(OpCodes.Ldfld, innerFieldBuilder);

                    foreach (var argIndex in Enumerable.Range(1, paramz.Length))
                    {
                        methodIl.Emit(OpCodes.Ldc_I4_S, argIndex);
                        methodIl.Emit(OpCodes.Ldarg_S);
                    }

                    methodIl.Emit(OpCodes.Call, method);
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
                        methodIl.Emit(OpCodes.Call, valueMethod);
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

                        methodIl.Emit(OpCodes.Ldftn, method);
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

            var generatedType = typeBuilder.CreateType();
            // TODO: InvalidProgramException: Common Language Runtime detected an invalid program.
            var instance = generatedType.GetConstructors().Single().Invoke(ArrayOf((object) inner));
            return (A) instance;
        }
    }
}
