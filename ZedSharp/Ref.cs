namespace ZedSharp
{
    public static class Ref
    {
        public static Ref<A> Of<A>(A val)
        {
            return new Ref<A>(val);
        }

        public static Maybe<A> ToMaybe<A>(this Ref<A> r)
        {
            return r == null ? Maybe.None<A>() : Maybe.Of(r.Value);
        }

        public static A OrElse<A>(this Ref<A> r, A val)
        {
            return r == null ? val : r.Value;
        }
    }

    public class Ref<A>
    {
        public Ref(A val)
        {
            Value = val;
        }

        public A Value { get; private set; }
    }
}
