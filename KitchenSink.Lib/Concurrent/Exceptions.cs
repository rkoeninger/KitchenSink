using System;

namespace KitchenSink.Concurrent
{
    public class InvalidSubtypeException : Exception
    {
        public InvalidSubtypeException() : base("Class hierarchy is supposed to be closed") { }
    }
}
