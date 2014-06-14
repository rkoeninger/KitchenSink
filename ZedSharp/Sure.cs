using System;

namespace ZedSharp
{
    public static class Sure
    {
        public static Sure<A> Of<A>(A val)
        {
            return new Sure<A>(val);
        }
    }

    /// <summary>
    /// A null-safe wrapper.
    /// References to Sures and their Values are guaranteed to be non-null.
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public struct Sure<A>
    {
        public static implicit operator Sure<A>(A val)
        {
            return new Sure<A>(val);
        }

        public static implicit operator A(Sure<A> sure)
        {
            return sure.Value;
        }

        public static implicit operator Unsure<A>(Sure<A> sure)
        {
            return Unsure.Of(sure.Value);
        }

        internal Sure(A val) : this()
        {
            if (val == null)
                throw new ArgumentNullException("Attempted to create a Sure<" + GetType().GetGenericArguments()[0].Name + "> with a null value");

            Value = val;
        }

        public A Value { get; private set; }

        public Unsure<B> Map<B>(Func<A, B> f)
        {
            var val = Value;
            return Unsure.Try(() => f(val));
        }

        public Unsure<A> Filter(Func<A, bool> f)
        {
            return Unsure.If(Value, f);
        }
        
        public override string ToString()
        {
            return Value.ToString();
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode() ^ (int)0x06c6c6c6;
        }

        public override bool Equals(object other)
        {
            if (other == null || !(other is Sure<A>))
                return false;

            var that = (Unsure<A>) other;
 
            return Object.Equals(this.Value, that.Value);
        }
    }
}
