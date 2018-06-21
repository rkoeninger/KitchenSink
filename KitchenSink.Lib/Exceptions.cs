using System;

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
}
