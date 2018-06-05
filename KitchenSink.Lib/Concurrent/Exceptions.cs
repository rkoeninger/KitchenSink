using System;

namespace KitchenSink.Concurrent
{
    public class AgentDisposedException : ObjectDisposedException
    {
        public AgentDisposedException() : base("Agent has already been disposed")
        {
        }
    }

    public class OutsideTranScopeException : Exception
    {
        public OutsideTranScopeException() : base("Must provide a Tran or be in scope of an ambient Tran")
        {
        }
    }
}
