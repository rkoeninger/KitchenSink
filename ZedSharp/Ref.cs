using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedSharp
{
    public static class Ref
    {
        public static Ref<A> Of<A>(A val)
        {
            return new Ref<A>(val);
        }

        public static Unsure<A> ToUnsure<A>(this Ref<A> r)
        {
            return r == null ? Unsure.None<A>() : Unsure.Of(r.Value);
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
