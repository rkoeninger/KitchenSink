using System;

namespace KitchenSink.Injection
{
    public class InvalidImplementationException : Exception
    {
        public InvalidImplementationException(Type contractType, Type implType)
            : base($"Object of type {implType} does not implement {contractType}")
        {
            ContractType = contractType;
        }

        public Type ContractType { get; }
    }

    public class ImplementationUnresolvedException : Exception
    {
        public ImplementationUnresolvedException(Type contractType)
            : base($"No implementation found for {contractType}")
        {
            ContractType = contractType;
        }

        public Type ContractType { get; }
    }

    public class ImplementationReliabilityException : Exception
    {
        public ImplementationReliabilityException(Type contractType, Type implType, Type argType)
            : base($"MultiUse class ({implType}) cannot depend on SingleUse class ({argType})")
        {
            ContractType = contractType;
            ImplementationType = implType;
            ArgumentType = argType;
        }

        public Type ContractType { get; }
        public Type ImplementationType { get; }
        public Type ArgumentType { get; }
    }

    public class MultipleConstructorsException : Exception
    {
        public MultipleConstructorsException(Type implType, int ctorCount)
            : base($"Type {implType} must have exactly 1 constructor, but has {ctorCount}")
        {
            ImplementationType = implType;
        }
        
        public Type ImplementationType { get; }
    }
}
