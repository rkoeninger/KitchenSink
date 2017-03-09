using System;
using System.Collections.Generic;
using System.Linq;
using KitchenSink.Extensions;
using static KitchenSink.Operators;

namespace KitchenSink
{
    public static class Maybe
    {
        public static Maybe<A> If<A>(A val, Func<A, bool> f)
        {
            return If(val, f, x => x);
        }

        public static Maybe<B> If<A, B>(A val, Func<A, bool> f, Func<A, B> convert)
        {
            return f(val) ? MaybeOf(convert(val)) : None<B>();
        }

        public static Lazy<Maybe<B>> LazyIf<A, B>(A val, Func<A, bool> f, Func<A, B> selector)
        {
            return new Lazy<Maybe<B>>(() => If(val, f, selector));
        }

        public static Maybe<A> Flatten<A>(this Maybe<Maybe<A>> maybe)
        {
            return maybe.HasValue ? maybe.Value : None<A>();
        }

        public static Maybe<Tuple<A, B>> Zip<A, B>(this Maybe<A> x, Maybe<B> y) =>
            x.Join(y, TupleOf);

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

            return array.Any(x => !x.HasValue) ? None<IEnumerable<A>>() : MaybeOf(array.WhereSome());
        }

        public static Func<A, Maybe<C>> Compose<A, B, C>(this Func<A, Maybe<B>> f, Func<B, Maybe<C>> g)
        {
            return a => f(a).SelectMany(g);
        }

        public static Func<A, Maybe<B>> Demote<A, B>(this Maybe<Func<A, B>> maybe)
        {
            if (maybe.HasValue)
                return x => MaybeOf(maybe.Value(x));

            return _ => None<B>();
        }

        public static int Compare<A>(Maybe<A> x, Maybe<A> y) where A : IComparable<A>
        {
            if (!x.HasValue && !y.HasValue)
                return 0;

            if (!x.HasValue && y.HasValue)
                return -1;

            if (x.HasValue && !y.HasValue)
                return 1;

            return Comparer<A>.Default.Compare(x.Value, y.Value);
        }
    }
    
    /// <summary>
    /// A null-encapsulating wrapper.
    /// A Maybe might not have a value, but a reference to an Maybe will not itself be null.
    /// </summary>
    public struct Maybe<A>
    {
        internal static readonly Maybe<A> None = default(Maybe<A>);

        public static implicit operator Maybe<A>(A val)
        {
            return MaybeOf(val);
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

        internal Maybe(A value, bool? hasValue = null) : this()
        {
            Value = value;
            HasValue = hasValue ?? NonNull(value);
        }

        internal A Value { get; }
        public bool HasValue { get; }

        public Type InnerType => typeof(A);

        public Maybe<B> Cast<B>() => HasValue && Value is B ? Some((B) (object) Value) : None<B>();
        public Maybe<B> Select<B>(Func<A, B> f) => HasValue ? Some(f(Value)) : None<B>();
        public Maybe<B> SelectMany<B>(Func<A, Maybe<B>> f) => Select(f).Flatten();
        public Maybe<A> Where(Func<A, bool> f) => HasValue && f(Value) ? this : None<A>();

        public Maybe<C> Join<B, C, K>(
            Maybe<B> inner,
            Func<A, K> outerKeySelector,
            Func<B, K> innerKeySelector,
            Func<A, B, C> resultSelector,
            IEqualityComparer<K> comparer)
        {
            return HasValue
                   && inner.HasValue
                   && comparer.Equals(outerKeySelector(Value), innerKeySelector(inner.Value))
                ? MaybeOf(resultSelector(Value, inner.Value))
                : None<C>();
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

        public B Branch<B>(Func<A, B> forSome, Func<B> forNone) =>
            HasValue ? forSome(Value) : forNone();
        public A OrElse(A other) => HasValue ? Value : other;
        public A OrElseDo(Func<A> f) => HasValue ? Value : f();
        public Maybe<A> OrDo(Func<A> f) => HasValue ? this : Some(f());
        public Maybe<A> OrDo(Func<Maybe<A>> f) => HasValue ? this : f();
        public Maybe<A> Or(Maybe<A> maybe) => HasValue ? this : maybe;

        public Maybe<A> OrThrow(string message)
        {
            if (HasValue)
            {
                return this;
            }

            throw new Exception(message);
        }

        public Maybe<A> OrThrow(Exception e)
        {
            if (HasValue)
            {
                return this;
            }

            throw e;
        }

        public Maybe<A> OrThrow(Func<Exception> f)
        {
            if (HasValue)
            {
                return this;
            }
            
            throw f();
        }

        public A OrElseThrow(string message)
        {
            if (HasValue)
            {
                return Value;
            }

            throw new Exception(message);
        }

        public A OrElseThrow(Exception e)
        {
            if (HasValue)
            {
                return Value;
            }

            throw e;
        }

        public A OrElseThrow(Func<Exception> f)
        {
            if (HasValue)
            {
                return Value;
            }

            throw f();
        }

        public void ForEach(Action<A> f)
        {
            if (HasValue)
            {
                f(Value);
            }
        }

        public List<A> ToList() => HasValue ? ListOf(Value) : new List<A>();
        public A[] ToArray() => HasValue ? ArrayOf(Value) : new A[0];
        public override string ToString() => HasValue ? Str(Value) : "None";
        public override int GetHashCode() => HasValue ? Hash(Value) : 1;

        public override bool Equals(object other)
        {
            if (!(other is Maybe<A>))
                return false;

            var that = (Maybe<A>) other;
            return (!HasValue && !that.HasValue) || (HasValue && that.HasValue && Equals(Value, that.Value));
        }
    }
}
