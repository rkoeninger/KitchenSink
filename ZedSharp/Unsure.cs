using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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

        public static Unsure<XDocument> ToXml(this String s)
        {
            return Try(() => XDocument.Parse(s));
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

        public static Unsure<IEnumerable<A>> NonEmpty<A>(Unsure<IEnumerable<A>> unsure)
        {
            return unsure.Where(x => x.Any());
        }

        public static Unsure<List<A>> NonEmpty<A>(Unsure<List<A>> unsure)
        {
            return unsure.Where(x => x.Count > 0);
        }

        public static Unsure<String> NonEmpy(Unsure<String> unsure)
        {
            return unsure.Where(x => ! String.IsNullOrEmpty(x));
        }

        public static Unsure<String> NonWhitespace(Unsure<String> unsure)
        {
            return unsure.Where(x => ! String.IsNullOrWhiteSpace(x));
        }

        public static Unsure<int> NonNegative(Unsure<int> unsure)
        {
            return unsure.Where(x => x >= 0);
        }

        public static Unsure<int> Positive(Unsure<int> unsure)
        {
            return unsure.Where(x => x > 0);
        }
    }
    
    /// <summary>
    /// A null-encapsulating wrapper.
    /// An Unsure might not have a value, not a reference to an Unsure can not be null.
    /// </summary>
    /// <typeparam name="A"></typeparam>
    public struct Unsure<A>
    {
        public static implicit operator Unsure<A>(A val)
        {
            return Unsure.Of(val);
        }

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

        public Type InnerType { get { return typeof(A); } }

        public Unsure<B> Select<B>(Func<A, B> f)
        {
            var val = Value;
            return HasValue ? Unsure.Try(() => f(val)) : HasError ? Unsure.Error<B>(Error) : Unsure.None<B>();
        }

        public Unsure<B> SelectMany<B>(Func<A, Unsure<B>> f)
        {
            return Select(f).Flatten();
        }

        public Unsure<A> Where(Func<A, bool> f)
        {
            return HasValue ? Unsure.If(Value, f) : this;
        }

        public Unsure<C> Join<B, C>(Unsure<B> that, Func<A, B, C> f)
        {
            var val = Value;
            return HasValue ? that.Select(x => f(val, x)) :
                HasError ? Unsure.Error<C>(Error) : Unsure.None<C>();
        }

        public Unsure<B> Cast<B>()
        {
            var val = Value;
            return Unsure.Try(() => (B) (Object) val);
        }

        public Unsure<B> As<B>() where B : class
        {
            return HasError ? Unsure.Error<B>(Error) : Unsure.Of(Value as B);
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
        
        public Unsure<A> ForEach(Action<A> f)
        {
            if (HasValue)
                f(Value);

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

        public override string ToString()
        {
            return HasValue ? Value.ToString() :
                HasError ? Error.ToString() :
                "None";
        }

        public override int GetHashCode()
        {
            return HasValue ? Value.GetHashCode() ^ (int)0x0a5a5a5a :
               HasError ? Error.GetHashCode() ^ (int)0x05a5a5a5 :
               1;
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(other, this))
                return true;

            if (other == null || !(other is Unsure<A>))
                return false;

            var that = (Unsure<A>) other;
 
            return (this.HasValue && that.HasValue && Object.Equals(this.Value, that.Value))
                || (this.HasError && that.HasError && Object.Equals(this.Error, that.Error))
                || (!this.HasValue && !that.HasValue && !this.HasError && !that.HasError);
        }
    }
}
