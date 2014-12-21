using System;

namespace ZedSharp
{
    /// <summary>
    /// Void is an empty placeholder type.
    /// An instance of Void can never be created.
    /// References will always be null.
    /// </summary>
    public sealed class Void
    {
        public static readonly Void It = null;

        private Void()
        {
            throw new InvalidOperationException("Void cannot be created");
        }
    }
}
