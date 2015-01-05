using System;

namespace ZedSharp
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
    public class DefaultImplementationOfAttribute : Attribute
    {
        /// <summary>Declares that this class or struct type is the default implementation of the specified interface type.</summary>
        public DefaultImplementationOfAttribute(Type @interface)
        {
            if (! @interface.IsInterface)
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
