using System;
using System.Collections.Generic;

namespace KitchenSink
{
    public class DynamicScopeStackException : InvalidOperationException
    {
        public DynamicScopeStackException(int expectedSize, int actualSize)
            : base($"Stack was expected to be size: {expectedSize}, but was: {actualSize}") { }
    }

    public class CloneNotSupportedException : NotSupportedException
    {
        public CloneNotSupportedException(Type type)
            : base($"The type {type} does not implement ICloneable, nor is it serializable.") { }
    }

    public class InvalidDriveLetterException : ArgumentException
    {
        public InvalidDriveLetterException(char letter)
            : base($"Should be a drive letter, but was '{letter}'") { }
    }

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

    public class RetryExhaustedException : AggregateException
    {
        public RetryExhaustedException(int count, IEnumerable<Exception> exceptions)
            : base($"Retry exhausted after {count} attempts", exceptions) { }
    }
}
