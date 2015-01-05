using System;

namespace ZedSharp
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class DefaultImplementationAttribute : Attribute
    {
        /// <summary>Declares the default implementation of this interface.</summary>
        public DefaultImplementationAttribute(Type @class)
        {
            if ((! (@class.IsClass || @class.IsValueType)) || @class.IsAbstract)
                throw new ArgumentException(@class + " is not a concrete class or struct type");

            ImplementingClass = @class;
        }

        public Type ImplementingClass { get; private set; }

        public bool IsProperlyDefinedOn(Type implementedInterface)
        {
            return ImplementingClass.IsAssignableTo(implementedInterface);
        }
    }
}
