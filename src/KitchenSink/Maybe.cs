using System;
using System.Collections.Generic;
using System.Linq;
using KitchenSink.Extensions;
using static KitchenSink.Operators;

namespace KitchenSink
{
    /// <summary>
    /// A null-encapsulating wrapper.
    /// A Maybe might not have a value, but a reference to an Maybe will not itself be null.
    /// </summary>
    public static class Maybe
    {
        public static Maybe<A> If<A>(bool b, A val) =>
            If(val, _ => b);

        public static Maybe<A> If<A>(bool b, Func<A> f) =>
            If(default(A), _ => b, _ => f());

        public static Maybe<A> If<A>(A val, Func<A, bool> f) =>
            If(val, f, x => x);

        public static Maybe<B> If<A, B>(A val, Func<A, bool> f, Func<A, B> convert) =>
            f(val) ? MaybeOf(convert(val)) : None<B>();

        public static Maybe<B> SelectMany<A, B>(this Maybe<A> m, Func<A, Maybe<B>> selector) =>
            m.Select(selector).Flatten();

        public static Maybe<C> SelectMany<A, B, C>(this Maybe<A> m, Func<A, Maybe<B>> k, Func<A, B, C> s) =>
            m.SelectMany(x => k(x).SelectMany(y => Some(s(x, y))));

        public static Lazy<Maybe<B>> LazyIf<A, B>(A val, Func<A, bool> f, Func<A, B> selector) =>
            new Lazy<Maybe<B>>(() => If(val, f, selector));

        public static Maybe<A> Flatten<A>(this Maybe<Maybe<A>> maybe) =>
            maybe.HasValue ? maybe.Value : None<A>();

        public static Maybe<(A, B)> Zip<A, B>(this Maybe<A> x, Maybe<B> y) => x.Join(y, TupleOf);

        public static Maybe<C> Join<A, B, C>(this Maybe<A> outer, Maybe<B> inner, Func<A, B, C> resultSelector) =>
            outer.Join(inner, _ => 0, _ => 0, resultSelector, EqualityComparer<int>.Default);

        public static Maybe<C> Join<A, B, C, K>(
            this Maybe<A> outer,
            Maybe<B> inner,
            Func<A, K> outerKeySelector,
            Func<B, K> innerKeySelector,
            Func<A, B, C> resultSelector) =>
            outer.Join(inner, outerKeySelector, innerKeySelector, resultSelector, EqualityComparer<K>.Default);

        public static Maybe<IEnumerable<A>> Sequence<A>(this IEnumerable<Maybe<A>> seq)
        {
            var array = seq.ToArray();

            return array.Any(x => !x.HasValue) ? None<IEnumerable<A>>() : MaybeOf(array.WhereSome());
        }

        public static Func<A, Maybe<C>> Compose<A, B, C>(this Func<A, Maybe<B>> f, Func<B, Maybe<C>> g) =>
            a => f(a).SelectMany(g);

        public static Func<A, Maybe<B>> Demote<A, B>(this Maybe<Func<A, B>> maybe) =>
            x => maybe.HasValue ? MaybeOf(maybe.Value(x)) : None<B>();

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
    public readonly struct Maybe<A> : IEquatable<Maybe<A>>
    {
        internal static readonly Maybe<A> None = default;

        public static implicit operator Maybe<A>(A val) => MaybeOf(val);

        public static Maybe<A> operator |(Maybe<A> x, Maybe<A> y) => x.Or(y);

        public static A operator |(Maybe<A> x, A y) => x.OrElse(y);

        public static bool operator ==(Maybe<A> x, Maybe<A> y) => Equals(x, y);

        public static bool operator !=(Maybe<A> x, Maybe<A> y) => !Equals(x, y);

        internal Maybe(A value, bool? hasValue = null) : this()
        {
            Value = value;
            HasValue = hasValue ?? NonNull(value);
        }

        internal A Value { get; }
        public bool HasValue { get; }

        public Type InnerType => typeof(A);

        public void Deconstruct(out A value, out bool hasValue)
        {
            value = Value;
            hasValue = HasValue;
        }

        public Maybe<B> Cast<B>() => HasValue && Value is B ? Some((B) (object) Value) : None<B>();

        public Maybe<B> OfType<B>() => Where(Is<A, B>()).Cast<B>();

        public Maybe<B> Select<B>(Func<A, B> f) => HasValue ? Some(f(Value)) : None<B>();

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

        public bool TryGetValue(out A value)
        {
            value = Value;
            return HasValue;
        }

        public A OrElse(A other) => HasValue ? Value : other;

        public A OrElseDo(Func<A> f) => HasValue ? Value : f();

        public Maybe<A> OrDo(Func<A> f) => HasValue ? this : Some(f());

        public Maybe<A> OrDo(Func<Maybe<A>> f) => HasValue ? this : f();

        public Maybe<A> Or(Maybe<A> maybe) => HasValue ? this : maybe;

        public Maybe<A> OrThrow(string message) => OrThrow(() => new Exception(message));

        public Maybe<A> OrThrow(Exception e) => OrThrow(Const(e));

        public Maybe<A> OrThrow(Func<Exception> f) => HasValue ? this : throw f();

        public Maybe<A> OrThrow<E>() where E : Exception, new() => OrThrow(new E());

        public A OrElseThrow(string message) => OrElseThrow(() => new Exception(message));

        public A OrElseThrow(Exception e) => OrElseThrow(Const(e));

        public A OrElseThrow<E>() where E : Exception, new() => OrElseThrow(new E());

        public A OrElseThrow(Func<Exception> f) => HasValue ? Value : throw f();

        public Maybe<A> AndOr(Maybe<A> maybe, Func<A, A, A> reducer) =>
            HasValue && maybe.HasValue
                ? Some(reducer(Value, maybe.Value))
                : Or(maybe);

        public void ForEach(Action<A> f)
        {
            if (HasValue)
            {
                f(Value);
            }
        }

        public Maybe<Unit> Reverse() =>
            HasValue ? None<Unit>() : Some(Unit.It);

        public IEnumerable<A> AsEnumerable()
        {
            if (HasValue)
            {
                yield return Value;
            }
        }

        public List<A> ToList() => HasValue ? ListOf(Value) : new List<A>();
        public A[] ToArray() => HasValue ? ArrayOf(Value) : Array.Empty<A>();
        public override string ToString() => HasValue ? Str(Value) : "None";

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<A>.Default.GetHashCode(Value) * 397) ^ HasValue.GetHashCode();
            }
        }

        public bool Equals(Maybe<A> other) =>
            EqualityComparer<A>.Default.Equals(Value, other.Value) && HasValue == other.HasValue;

        public override bool Equals(object other) =>
            other is Maybe<A> maybe && Equals(maybe);
    }
}
