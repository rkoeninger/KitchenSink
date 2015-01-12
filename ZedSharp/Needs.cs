using System;
using System.Linq;

namespace ZedSharp
{
    /// <summary>Simple IoC container that makes use of DefaultImplementation and DefaultImplementationOf attributes.</summary>
    public class Needs
    {
        public static Needs Of(params Object[] vals)
        {
            var needs = new Needs();

            foreach (var val in vals)
                needs.Tree.Set(val.GetType(), val);

            return needs;
        }

        public Needs()
        {
            Tree = new TypeTree<Object>();
        }

        private readonly TypeTree<Object> Tree;

        public Needs Set<T>(Object impl)
        {
            Tree.Set<T>(impl);
            return this;
        }

        public Maybe<T> Get<T>() where T : class
        {
            var t = typeof(T);
            return Tree.Get(t)
                .OrEvalMany(t, GetDefaultImpl)
                .Cast<T>();
        }

        private static Maybe<Object> GetDefaultImpl(Type @interface)
        {
            return GetDeclaredImplementingClass(@interface)
                .OrEvalMany(@interface, FindDeclaringImplementingClass)
                .Select(Activator.CreateInstance);
        }

        private static Maybe<Type> GetDeclaredImplementingClass(Type @interface)
        {
            return @interface.GetAttribute<DefaultImplementationAttribute>()
                .Select(x => x.ImplementingClass);
        }

        private static Maybe<Type> FindDeclaringImplementingClass(Type @interface)
        {
            var impls = Types.All(t => t.HasAttribute<DefaultImplementationOfAttribute>(a => a.ImplementedInterface == @interface)).ToList();

            if (impls.Count > 1)
                throw new Exception("Multiple implementations found: " + impls.Concat(", "));

            return impls.SingleMaybe();
        }
    }

    /// <summary>Declares the default implementation of this interface.</summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class DefaultImplementationAttribute : Attribute
    {
        public DefaultImplementationAttribute(Type @class)
        {
            if ((!(@class.IsClass || @class.IsValueType)) || @class.IsAbstract)
                throw new ArgumentException(@class + " is not a concrete class or struct type");

            ImplementingClass = @class;
        }

        public Type ImplementingClass { get; private set; }

        public bool IsProperlyDefinedOn(Type implementedInterface)
        {
            return ImplementingClass.IsAssignableTo(implementedInterface);
        }
    }

    /// <summary>Declares that this class or struct type is the default implementation of the specified interface type.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
    public class DefaultImplementationOfAttribute : Attribute
    {
        public DefaultImplementationOfAttribute(Type @interface)
        {
            if (!@interface.IsInterface)
                throw new ArgumentException(@interface + " is not an interface type");

            ImplementedInterface = @interface;
        }

        public Type ImplementedInterface { get; private set; }

        public bool IsProperlyDefinedOn(Type implementingClass)
        {
            return implementingClass.IsAssignableTo(ImplementedInterface);
        }
    }
}
