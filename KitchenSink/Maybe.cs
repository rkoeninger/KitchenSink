using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using KitchenSink.Collections;
using static KitchenSink.Collections.ConstructionOperators;

namespace KitchenSink
{
    public static class Maybe
    {
        public static Maybe<A> Of<A>(A? nullable) where A : struct
        {
            return nullable.HasValue ? new Maybe<A>(nullable.Value) : Maybe<A>.None;
        }

        public static Maybe<A> Of<A>(A val)
        {
            return new Maybe<A>(val);
        }

        public static Maybe<A> Some<A>(A val)
        {
            if (val.IsNull())
                throw new ArgumentNullException("Can't create Maybe.IsSome<" + typeof(A) + "> with null value");

            return new Maybe<A>(val);
        }

        public static Maybe<A> Try<A>(Func<A> f)
        {
            try
            {
                return Of(f());
            }
            catch
            {
                return Maybe<A>.None;
            }
        }

        public static Maybe<A> If<A>(A val, Func<A, bool> f)
        {
            return If(val, f, x => x);
        }

        public static Maybe<B> If<A, B>(A val, Func<A, bool> f, Func<A, B> convert)
        {
            return f(val) ? Of(convert(val)) : Maybe<B>.None;
        }

        public static Lazy<Maybe<B>> LazyIf<A, B>(A val, Func<A, bool> f, Func<A, B> selector)
        {
            return new Lazy<Maybe<B>>(() => If(val, f, selector));
        }

        public static Maybe<int> ToInt(this string s)
        {
            int i;
            return int.TryParse(s, out i) ? Of(i) : Maybe<int>.None;
        }

        public static Maybe<double> ToDouble(this string s)
        {
            double d;
            return double.TryParse(s, out d) ? Of(d) : Maybe<double>.None;
        }

        public static Maybe<XDocument> ToXml(this string s)
        {
            return Try(() => XDocument.Parse(s));
        }

        public static Maybe<A> GetMaybe<A>(this IReadOnlyList<A> list, int index)
        {
            return Try(() => list[index]);
        }

        public static Maybe<A> GetMaybe<A>(this IList<A> list, int index)
        {
            return Try(() => list[index]);
        }

        public static Maybe<B> GetMaybe<A, B>(this IReadOnlyDictionary<A, B> dict, A key)
        {
            return Try(() => dict[key]);
        }

        public static Maybe<B> GetMaybe<A, B>(this IDictionary<A, B> dict, A key)
        {
            return Try(() => dict[key]);
        }

        public static Maybe<B> GetMaybe<A, B>(this ConcurrentDictionary<A, B> dict, A key)
        {
            return Try(() => dict[key]);
        }

        public static Maybe<A> FirstMaybe<A>(this IEnumerable<A> seq)
        {
            foreach (var item in seq)
                return Some(item);

            return Maybe<A>.None;
        }

        public static Maybe<A> FirstMaybe<A>(this IEnumerable<A> seq, Func<A, bool> predicate)
        {
            foreach (var item in seq.Where(predicate))
                return Some(item);

            return Maybe<A>.None;
        }

        public static Maybe<A> FirstSome<A>(this IEnumerable<Maybe<A>> seq)
        {
            foreach (var item in seq)
                if (item.HasValue)
                    return item;

            return Maybe<A>.None;
        }

        public static Maybe<B> FirstSome<A, B>(this IEnumerable<A> seq, Func<A, Maybe<B>> selector)
        {
            return FirstSome(seq.Select(selector));
        }

        public static Maybe<A> LastMaybe<A>(this IEnumerable<A> seq)
        {
            return Try(seq.Last);
        }

        public static Maybe<A> SingleMaybe<A>(this IEnumerable<A> seq)
        {
            return Try(seq.Single);
        }

        public static Maybe<A> ElementAtMaybe<A>(this IEnumerable<A> seq, int index)
        {
            return Try(() => seq.ElementAt(index));
        }

        public static Maybe<A> Flatten<A>(this Maybe<Maybe<A>> maybe)
        {
            return maybe.HasValue ? maybe.Value : Maybe<A>.None;
        }

        public static IEnumerable<A> Flatten<A>(this IEnumerable<Maybe<A>> seq)
        {
            return seq.Where(x => x.HasValue).Select(x => x.Value);
        }

        public static IEnumerable<B> SelectMany<A, B>(this IEnumerable<A> seq, Func<A, Maybe<B>> f)
        {
            return seq.Select(f).Flatten();
        }

        public static Maybe<C> Join<A, B, C>(this Maybe<A> outer, Maybe<B> inner, Func<A, B, C> resultSelector)
        {
            return outer.Join(inner, _ => 0, _ => 0, resultSelector, EqualityComparer<int>.Default);
        }

        public static Maybe<C> Join<A, B, C, K>(this Maybe<A> outer, Maybe<B> inner, Func<A, K> outerKeySelector, Func<B, K> innerKeySelector, Func<A, B, C> resultSelector)
        {
            return outer.Join(inner, outerKeySelector, innerKeySelector, resultSelector, EqualityComparer<K>.Default);
        }

        public static Maybe<IEnumerable<A>> Sequence<A>(this IEnumerable<Maybe<A>> seq)
        {
            var array = seq.ToArray();

            return array.Any(x => !x.HasValue) ? Maybe<IEnumerable<A>>.None : Of(array.Flatten());
        }

        public static Func<A, Maybe<C>> Compose<A, B, C>(this Func<A, Maybe<B>> f, Func<B, Maybe<C>> g)
        {
            return a => f(a).SelectMany(g);
        }

        public static Func<A, Maybe<B>> Demote<A, B>(this Maybe<Func<A, B>> maybe)
        {
            if (maybe.HasValue)
                return x => Of(maybe.Value(x));

            return _ => Maybe<B>.None;
        }

        public static Maybe<IEnumerable<A>> IsNotEmpty<A>(Maybe<IEnumerable<A>> maybe)
        {
            return maybe.Where(CollectionHelpers.NotEmpty);
        }

        public static Maybe<List<A>> IsNotEmpty<A>(Maybe<List<A>> maybe)
        {
            return maybe.Where(CollectionHelpers.NotEmpty);
        }

        public static Maybe<string> IsNotBlank(Maybe<string> maybe)
        {
            return maybe.Where(Strings.IsNotBlank);
        }

        public static Maybe<int> NotNeg(Maybe<int> maybe)
        {
            return maybe.Where(Z.NotNeg);
        }

        public static Maybe<int> Pos(Maybe<int> maybe)
        {
            return maybe.Where(Z.Pos);
        }

        public static A OrElseNull<A>(this Maybe<A> maybe) where A : class
        {
            return maybe.OrElse(null);
        }

        public static bool OrElseFalse(this Maybe<bool> maybe)
        {
            return maybe.OrElse(false);
        }

        public static int OrElseZero(this Maybe<int> maybe)
        {
            return maybe.OrElse(0);
        }

        public static string OrElseEmpty(this Maybe<string> maybe)
        {
            return maybe.OrElse("");
        }

        public static IEnumerable<A> OrElseEmpty<A>(this Maybe<IEnumerable<A>> maybe)
        {
            return maybe.OrElse(seqof<A>());
        }

        public static Maybe<bool> OrFalse(this Maybe<bool> maybe)
        {
            return maybe.Or(Of(false));
        }

        public static Maybe<int> OrZero(this Maybe<int> maybe)
        {
            return maybe.Or(Of(0));
        }

        public static Maybe<string> OrEmpty(this Maybe<string> maybe)
        {
            return maybe.Or(Of(""));
        }

        public static Maybe<IEnumerable<A>> OrEmpty<A>(this Maybe<IEnumerable<A>> maybe)
        {
            return maybe.Or(Of(seqof<A>()));
        }

        public static int Compare<A>(Maybe<A> x, Maybe<A> y) where A : IComparable<A>
        {
            if (x.HasValue == y.HasValue == false)
                return 0;

            if (x.HasValue && !y.HasValue)
                return 1;

            if (!x.HasValue && y.HasValue)
                return -1;

            return 0;
        }

        public static Maybe<R> All<A, R>(Maybe<A> ma, Func<A, R> f)
        {
            return ma.Select(f);
        }

        public static Maybe<R> All<A, B, R>(Maybe<A> ma, Maybe<B> mb, Func<A, B, R> f)
        {
            return ma.HasValue && mb.HasValue ? Some(f(ma.Value, mb.Value)) : Maybe<R>.None;
        }

        public static Maybe<R> All<A, B, C, R>(Maybe<A> ma, Maybe<B> mb, Maybe<C> mc, Func<A, B, C, R> f)
        {
            return ma.HasValue && mb.HasValue && mc.HasValue ? Some(f(ma.Value, mb.Value, mc.Value)) : Maybe<R>.None;
        }

        public static Maybe<R> All<A, B, C, D, R>(Maybe<A> ma, Maybe<B> mb, Maybe<C> mc, Maybe<D> md, Func<A, B, C, D, R> f)
        {
            return ma.HasValue && mb.HasValue && mc.HasValue && md.HasValue ? Some(f(ma.Value, mb.Value, mc.Value, md.Value)) : Maybe<R>.None;
        }

        public static Maybe<R> AllFlat<A, R>(Maybe<A> ma, Func<A, Maybe<R>> f)
        {
            return ma.SelectMany(f);
        }

        public static Maybe<R> AllFlat<A, B, R>(Maybe<A> ma, Maybe<B> mb, Func<A, B, Maybe<R>> f)
        {
            return ma.HasValue && mb.HasValue ? f(ma.Value, mb.Value) : Maybe<R>.None;
        }

        public static Maybe<R> AllFlat<A, B, C, R>(Maybe<A> ma, Maybe<B> mb, Maybe<C> mc, Func<A, B, C, Maybe<R>> f)
        {
            return ma.HasValue && mb.HasValue && mc.HasValue ? f(ma.Value, mb.Value, mc.Value) : Maybe<R>.None;
        }

        public static Maybe<R> AllFlat<A, B, C, D, R>(Maybe<A> ma, Maybe<B> mb, Maybe<C> mc, Maybe<D> md, Func<A, B, C, D, Maybe<R>> f)
        {
            return ma.HasValue && mb.HasValue && mc.HasValue && md.HasValue ? f(ma.Value, mb.Value, mc.Value, md.Value) : Maybe<R>.None;
        }
    }
    
    /// <summary>
    /// A null-encapsulating wrapper.
    /// A Maybe might not have a value, but a reference to an Maybe will not itself be null.
    /// </summary>
    public struct Maybe<A>
    {
        public static readonly Maybe<A> None = default(Maybe<A>);

        public static implicit operator Maybe<A>(A val)
        {
            return Maybe.Of(val);
        }

        public static Maybe<A> operator |(Maybe<A> x, Maybe<A> y)
        {
            return x.Or(y);
        }

        public static A operator |(Maybe<A> x, A y)
        {
            return x.OrElse(y);
        }

        public static bool operator ==(Maybe<A> x, Maybe<A> y)
        {
            return Equals(x, y);
        }

        public static bool operator !=(Maybe<A> x, Maybe<A> y)
        {
            return !Equals(x, y);
        }

        internal Maybe(A val) : this()
        {
            Value = val;
            HasValue = val.IsNotNull();
        }

        internal A Value { get; set; }
        public bool HasValue { get; }

        public Type InnerType => typeof(A);

        public Maybe<B> Select<B>(Func<A, B> f)
        {
            return HasValue ? Maybe.Of(f(Value)) : Maybe<B>.None;
        }

        public Maybe<B> TrySelect<B>(Func<A, B> f)
        {
            var val = Value;
            return HasValue ? Maybe.Try(() => f(val)) : Maybe<B>.None;
        }

        public Maybe<B> SelectMany<B>(Func<A, Maybe<B>> f)
        {
            return Select(f).Flatten();
        }

        public Maybe<B> TrySelectMany<B>(Func<A, Maybe<B>> f)
        {
            return TrySelect(f).Flatten();
        }

        public Maybe<A> Where(Func<A, bool> f)
        {
            return HasValue ? Maybe.If(Value, f) : this;
        }

        public Maybe<C> Join<B, C, K>(Maybe<B> inner, Func<A, K> outerKeySelector, Func<B, K> innerKeySelector, Func<A, B, C> resultSelector, IEqualityComparer<K> comparer)
        {
            return HasValue && inner.HasValue && comparer.Equals(outerKeySelector(Value), innerKeySelector(inner.Value))
                ? Maybe.Of(resultSelector(Value, inner.Value))
                : Maybe<C>.None;
        }

        /// <summary>Attempts cast. Propogates None and returns None if cast fails.</summary>
        public Maybe<B> Cast<B>()
        {
            return Maybe.If(Value, x => x is B, x => (B) (object) x);
        }

        public B Branch<B>(Func<A, B> forSome, Func<B> forNone)
        {
            return HasValue ? forSome(Value) : forNone();
        }

        public void Branch(Action<A> forSome, Action forNone)
        {
            if (HasValue)
            {
                forSome(Value);
            }
            else
            {
                forNone();
            }
        }

        private static readonly Maybe<A> Default = Maybe.Of(default(A));

        public Maybe<A> OrDefault()
        {
            return HasValue ? this : Default;
        }

        public A OrElseDefault()
        {
            return HasValue ? Value : default(A);
        }

        public A OrElse(A other)
        {
            return HasValue ? Value : other;
        }

        public A OrElseEval(Func<A> f)
        {
            return HasValue ? Value : f();
        }

        public A OrElseEval<B>(B key, Func<B, A> f)
        {
            return HasValue ? Value : f(key);
        }

        public Maybe<A> OrEval(Func<A> f)
        {
            return HasValue ? this : Maybe.Try(f);
        }

        public Maybe<A> OrEval(Func<Maybe<A>> f)
        {
            return HasValue ? this : f();
        }

        public Maybe<A> OrEvalMany<B>(B key, Func<B, Maybe<A>> f)
        {
            return HasValue ? this : f(key);
        }

        public Maybe<A> OrEvalMany(Func<Maybe<A>> f)
        {
            return HasValue ? this : Maybe.Try(f).Flatten();
        }

        public Maybe<A> OrIf<B>(B key, Func<B, bool> p, Func<B, A> f)
        {
            return HasValue ? this : p(key) ? Maybe.Of(f(key)) : None;
        }

        public Maybe<A> OrIf<B>(B key, Func<B, bool> p, Func<B, Maybe<A>> f)
        {
            return HasValue ? this : p(key) ? f(key) : None;
        }

        public Maybe<A> Or(Maybe<A> maybe)
        {
            return HasValue ? this : maybe;
        }

        public Maybe<A> OrThrow(string message)
        {
            if (HasValue)
                return this;

            throw new Exception(message);
        }

        public Maybe<A> OrThrow(Exception e)
        {
            if (HasValue)
                return this;

            throw e;
        }

        public Maybe<A> OrThrow(Func<Exception> f)
        {
            if (HasValue)
                return this;
            
            throw f();
        }

        public A OrElseThrow(string message)
        {
            if (HasValue)
                return Value;

            throw new Exception(message);
        }

        public A OrElseThrow(Exception e)
        {
            if (HasValue)
                return Value;

            throw e;
        }

        public A OrElseThrow(Func<Exception> f)
        {
            if (HasValue)
                return Value;

            throw f();
        }

        public Maybe<A> ForEach(Action<A> f)
        {
            if (HasValue)
                f(Value);

            return this;
        }

        public List<A> ToList()
        {
            return HasValue ? listof(Value) : new List<A>();
        }

        public A[] ToArray()
        {
            return HasValue ? arrayof(Value) : new A[0];
        }

        public override string ToString()
        {
            return HasValue ? Value.ToString() : "None";
        }

        public override int GetHashCode()
        {
            return HasValue ? Value.GetHashCode() ^ 0x0a5a5a5a : 1;
        }

        public override bool Equals(object other)
        {
            if (!(other is Maybe<A>))
                return false;

            var that = (Maybe<A>) other;
            return (!HasValue && !that.HasValue) || (HasValue && that.HasValue && Equals(Value, that.Value));
        }
    }
}
