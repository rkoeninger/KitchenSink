using System;
using KitchenSink.Extensions;
using static KitchenSink.Operators;

namespace KitchenSink
{
    /// <summary>
    /// A binary-posibility wrapper.
    /// An Either might have a Left value or a Right value.
    /// </summary>
    public static class Either
    {
        public static Either<A, B> If<A, B>(
            Func<bool> condition,
            Func<A> consequent,
            Func<B> alternative) =>
            condition()
            ? new Either<A, B>(true, consequent(), default)
            : new Either<A, B>(false, default, alternative());

        public static Either<A, E> Try<A, E>(Func<A> f, Func<E, E> g) where E : Exception
        {
            try
            {
                return f();
            }
            catch (E e)
            {
                return g(e);
            }
        }

        public static Maybe<E> Try<E>(Action f) where E : Exception => Try<Unit, E>(f.AsFunc(), Id).RightMaybe;

        public static Either<C, B> Select<A, B, C>(
            this Either<A, B> e,
            Func<A, C> selector) =>
            SelectLeft(e, selector);

        public static Either<C, B> SelectLeft<A, B, C>(
            this Either<A, B> e,
            Func<A, C> selector) =>
            e.IsLeft
            ? new Either<C, B>(true, selector(e.Left), default)
            : new Either<C, B>(false, default, e.Right);

        public static Either<A, C> SelectRight<A, B, C>(
            this Either<A, B> e,
            Func<B, C> selector) =>
            e.IsLeft
            ? new Either<A, C>(true, e.Left, default)
            : new Either<A, C>(false, default, selector(e.Right));

        public static A OrElseThrow<A, E>(this Either<A, E> e) where E : Exception =>
            e.IsLeft ? e.Left : throw new AggregateException(e.Right);
    }

    /// <summary>
    /// A binary-posibility wrapper.
    /// An Either might have a Left value or a Right value.
    /// </summary>
    public class Either<A, B>
    {
        public static implicit operator Either<A, B>(A val) => new Either<A, B>(true, val, default);

        public static implicit operator Either<A, B>(B val) => new Either<A, B>(false, default, val);

        public static bool operator ==(Either<A, B> x, Either<A, B> y) => Equals(x, y);

        public static bool operator !=(Either<A, B> x, Either<A, B> y) => !Equals(x, y);

        internal Either(bool isLeft, A left, B right)
        {
            IsLeft = isLeft;
            Left = left;
            Right = right;
        }

        public bool IsLeft { get; }
        public bool IsRight => !IsLeft;
        internal A Left { get; }
        internal B Right { get; }

        public Type LeftType => typeof(A);
        public Type RightType => typeof(B);

        public Maybe<A> LeftMaybe => IsLeft ? Some(Left) : None<A>();
        public Maybe<B> RightMaybe => IsRight ? Some(Right) : None<B>();

        public void Branch(Action<A> forLeft, Action<B> forRight)
        {
            if (IsLeft)
            {
                forLeft(Left);
            }
            else
            {
                forRight(Right);
            }
        }
        
        public C Branch<C>(Func<A, C> forLeft, Func<B, C> forRight) =>
            IsLeft ? forLeft(Left) : forRight(Right);

        public void ForEachLeft(Action<A> f)
        {
            if (IsLeft)
            {
                f(Left);
            }
        }

        public void ForEachRight(Action<B> f)
        {
            if (IsRight)
            {
                f(Right);
            }
        }

        public Either<B, A> Reverse() => Branch(RightOf<B, A>, LeftOf<B, A>);

        public override string ToString() => IsLeft ? $"Left({Left})" : $"Right({Right})";
        public override int GetHashCode() => IsLeft ? Left.GetHashCode() : Right.GetHashCode();

        public override bool Equals(object other)
        {
            if (!(other is Either<A, B>))
                return false;

            var that = (Either<A, B>) other;
            return IsLeft && that.IsLeft && Equals(Left, that.Left)
                || IsRight && that.IsRight && Equals(Right, that.Right);
        }
    }
}
