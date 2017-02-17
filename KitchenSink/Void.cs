﻿using System;

namespace KitchenSink
{
    /// <summary>
    /// Void is an empty placeholder type.
    /// An instance of Void can never be created.
    /// References will always be null.
    /// </summary>
    // ReSharper disable once ConvertToStaticClass
    public sealed class Void
    {
        public static readonly Void It = null;

        private Void()
        {
            throw new InvalidOperationException("Void cannot be created");
        }
    }
}