using static KitchenSink.Operators;

namespace KitchenSink
{
    /// <summary>
    /// Used to specialize another type.
    /// </summary>
    public abstract class NewType<A>
    {
        /// <summary>
        /// Value that this NewType wraps.
        /// </summary>
        public A Value { get; }

        protected NewType(A value)
        {
            Value = value;
        }

        /// <summary>
        /// Returns string representation of wrapped value, "" for null.
        /// </summary>
        public override string ToString()
        {
            return Str(Value);
        }

        /// <summary>
        /// Gets hash code for wrapped value, 0 for null.
        /// </summary>
        public override int GetHashCode()
        {
            return Hash(Value);
        }

        /// <summary>
        /// Checks for equality of this NewType against another NewType.
        /// Argument must be a NewType&lt;A&gt;.
        /// 5 will not be equal to NewType(5).
        /// </summary>
        public override bool Equals(object obj)
        {
            var that = obj as NewType<A>;
            return that != null && Equals(Value, that.Value);
        }
    }
}
