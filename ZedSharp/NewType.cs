using System;

namespace ZedSharp
{
    /// <summary>
    /// Used to specialize another type.
    /// </summary>
    public abstract class NewType<A>
    {
        public A Value { get; private set; }

        protected NewType(A value)
        {
            Value = value;
        }

        /// <summary>
        /// Returns string representation of wrapped value, "" for null.
        /// </summary>
        public override String ToString()
        {
            return Z.Str(Value);
        }

        /// <summary>
        /// Gets hash code for wrapped value, 0 for null.
        /// </summary>
        public override int GetHashCode()
        {
            return Z.Hash(Value);
        }

        /// <summary>
        /// Checks for equality of this NewType against another NewType.
        /// Argument must be a NewType&lt;A&gt;.
        /// 5 will not be equal to NewType(5).
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is NewType<A> && Equals(Value, ((NewType<A>) obj).Value);
        }
    }
}
