using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using KitchenSink.Extensions;
using static KitchenSink.Operators;

namespace KitchenSink
{
    public static class AutoTrace
    {
        private static readonly ConcurrentDictionary<Type, Type> tracedTypes = new ConcurrentDictionary<Type, Type>();

        public static A Build<A>(
            A inner,
            Action<A, string> begin,
            Action<A, string> end) where A : class
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

            var generatedType = tracedTypes.GetOrAdd(typeof(A), Build(begin, end));
            return (A)Activator.CreateInstance(generatedType, inner);
        }

        private static Func<Type, Type> Build<A>(
            Action<A, string> begin,
            Action<A, string> end) => interfaceType =>
        {
            var beginTraceMethod = begin.Method;
            var endTraceMethod = end.Method;
            var noise = Guid.NewGuid().ToString().Substring(0, 8);
            var assemblyName = new AssemblyName($"{interfaceType.Name}_Assembly_{noise}");
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            assemblyBuilder.SetCustomAttribute(MakeCompilerGeneratedAttribute());
            var typeName = $"{interfaceType.Name}_Traced_{noise}";
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
            ctorIl.Emit(OpCodes.Ret);

            foreach (var method in interfaceType.GetMethods())
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
                methodIl.Emit(OpCodes.Ldarg_0);
                methodIl.Emit(OpCodes.Ldfld, innerFieldBuilder);
                methodIl.Emit(OpCodes.Ldstr, method.Name);
                // make array of arguments
                //methodIl.Emit(OpCodes.Ldc_I4, arity);
                //methodIl.Emit(OpCodes.Newarr, typeof(object));
                EmitCall(methodIl, beginTraceMethod);

                if (method.ReturnType == typeof(void))
                {
                    methodIl.Emit(OpCodes.Ldarg_0);
                    methodIl.Emit(OpCodes.Ldfld, innerFieldBuilder);
                    1.ToIncluding(arity).ForEach(i => methodIl.Emit(OpCodes.Ldarg_S, i));
                    EmitCall(methodIl, method);
                }
                else
                {
                    methodIl.Emit(OpCodes.Ldarg_0);
                    methodIl.Emit(OpCodes.Ldfld, innerFieldBuilder);
                    1.ToIncluding(arity).ForEach(i => methodIl.Emit(OpCodes.Ldarg_S, i));
                    EmitCall(methodIl, method);
                }

                methodIl.Emit(OpCodes.Ret);
            }

            return interfaceType;
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
