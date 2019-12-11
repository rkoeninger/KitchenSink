using System;

namespace KitchenSink
{
    /// <summary>
    /// Void is an empty placeholder type.
    /// An instance of Void can never be created.
    /// References will always be null.
    /// </summary>
    // ReSharper disable once ConvertToStaticClass
    // must be non-static so it can be used as a type parameter.
    public sealed class Void
    {
        /// <summary>
        /// The singleton "instance" of Void that is always null.
        /// </summary>
        public static readonly Void It = default;

        private Void() => throw new InvalidOperationException("Void cannot be created");
    }
}
