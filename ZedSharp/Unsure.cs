using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedSharp
{
    public static class Unsure
    {
        public static Unsure<A> Of<A>(Sure<A> sure)
        {
            return Of(sure.Value);
        }

        public static Unsure<A> Of<A>(Nullable<A> nullable) where A : struct
        {
            return nullable.HasValue ? new Unsure<A>(nullable.Value) : new Unsure<A>();
        }

        public static Unsure<A> Of<A>(A val)
        {
            return new Unsure<A>(val);
        }

        public static Unsure<A> Error<A>(Exception e)
        {
            return new Unsure<A>(e);
        }

        public static Unsure<A> None<A>()
        {
            return new Unsure<A>();
        }

        public static Unsure<A> Try<A>(Func<A> f)
        {
            try
            {
                return Of(f());
            }
            catch (Exception e)
            {
                return Error<A>(e);
            }
        }

        public static Unsure<A> If<A>(A val, Func<A, bool> f)
        {
            try
            {
                return f(val) ? Of(val) : None<A>();
            }
            catch (Exception e)
            {
                return Error<A>(e);
            }
        }

        public static Unsure<A> Flatten<A>(this Unsure<Unsure<A>> unsure)
        {
            return unsure.HasValue ? unsure.Value : None<A>();
        }

        public static Unsure<Int32> ToInt(this String s)
        {
            int i;
            return Int32.TryParse(s, out i) ? Unsure.Of(i) : Unsure.None<Int32>();
        }

        public static Unsure<Double> ToDouble(this String s)
        {
            double d;
            return Double.TryParse(s, out d) ? Unsure.Of(d) : Unsure.None<Double>();
        }

        public static Unsure<A> Get<A>(this IList<A> list, int index)
        {
            return Try(() => list[index]);
        }

        public static Unsure<B> Get<A, B>(this IDictionary<A, B> dict, A key)
        {
            return Try(() => dict[key]);
        }

        public static Unsure<A> UnsureFirst<A>(this IEnumerable<A> seq)
        {
            return Try(() => seq.First());
        }

        public static Unsure<A> UnsureLast<A>(this IEnumerable<A> seq)
        {
            return Try(() => seq.Last());
        }

        public static Unsure<A> UnsureSingle<A>(this IEnumerable<A> seq)
        {
            return Try(() => seq.Single());
        }

        public static Unsure<A> UnsureElementAt<A>(this IEnumerable<A> seq, int index)
        {
            return Try(() => seq.ElementAt(index));
        }

        public static IEnumerable<A> WhereSure<A>(this IEnumerable<Unsure<A>> seq)
        {
            foreach (var x in seq)
                if (x.HasValue)
                    yield return x.Value;
        }

        public static Unsure<IEnumerable<A>> Sequence<A>(this IEnumerable<Unsure<A>> seq)
        {
            var list = new List<A>();

            foreach (var x in seq)
                if (x.HasValue)
                    list.Add(x.Value);
                else
                    return Unsure.None<IEnumerable<A>>();

            return Unsure.Of(list).Cast<IEnumerable<A>>();
        }
    }
    
    /// <summary>
    /// A null-encapsulating wrapper.
    /// An Unsure might not have a value, not a reference to an Unsure can not be null.
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public struct Unsure<A>
    {
        internal Unsure(A val) : this()
        {
            Value = val;
            HasValue = val != null;
            Error = null;
            HasError = false;
        }

        internal Unsure(Exception e) : this()
        {
            Value = default(A);
            HasValue = false;
            Error = e;
            HasError = e != null;
        }

        internal A Value { get; set; }
        public bool HasValue { get; private set; }
        internal Exception Error { get; set; }
        public bool HasError { get; private set; }

        public Unsure<B> Map<B>(Func<A, B> f)
        {
            var val = Value;
            return HasValue ? Unsure.Try(() => f(val)) : HasError ? Unsure.Error<B>(Error) : Unsure.None<B>();
        }

        public Unsure<B> FlatMap<B>(Func<A, Unsure<B>> f)
        {
            return Map(f).Flatten();
        }

        public Unsure<A> Filter(Func<A, bool> f)
        {
            return HasValue ? Unsure.If(Value, f) : this;
        }

        public Unsure<B> Cast<B>()
        {
            var val = Value;
            return Unsure.Try(() => (B) (Object) val);
        }

        public Sure<A> OrElse(Sure<A> sure)
        {
            return HasValue ? Sure.Of(Value) : sure;
        }

        public Unsure<A> Or(Unsure<A> sure)
        {
            return HasValue ? this : sure;
        }

        public Unsure<A> Or(Exception e)
        {
            return HasValue ? this : Unsure.Error<A>(e);
        }

        public Unsure<Exception> UnsureError()
        {
            return HasError ? Unsure.Of(Error) : Unsure.None<Exception>();
        }

        public Unsure<A> Throw()
        {
            if (HasError)
                throw Error;

            return this;
        }

        public List<A> ToList()
        {
            return HasValue ? new List<A>() { Value } : new List<A>();
        }

        public A[] ToArray()
        {
            return HasValue ? new A[] { Value } : new A[0];
        }
    }
}
