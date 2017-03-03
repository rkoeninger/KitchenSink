using System;

namespace KitchenSink.Injection
{
    /// <summary>
    /// Indicates that a class/component is not thread safe, or has transient state
    /// and is only good for one time use.
    /// Multi-use classes cannot depend on single-use classes.
    /// Classes are multi-use by default.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SingleUse : Attribute
    {
    }
}
