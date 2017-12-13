using System;
using static KitchenSink.Operators;

namespace KitchenSink
{
    public static class Either
    {
        public static Either<A, B> If<A, B>(
            Func<bool> condition,
            Func<A> consequent,
            Func<B> alternative) =>
            condition()
            ? new Either<A, B>(true, consequent(), default)
            : new Either<A, B>(false, default, alternative());

        public static Either<C, B> Select<A, B, C>(
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
    }

    public class Either<A, B>
    {
        public static implicit operator Either<A, B>(A val)
        {
            return new Either<A, B>(true, val, default);
        }

        public static bool operator ==(Either<A, B> x, Either<A, B> y) => Equals(x, y);

        public static bool operator !=(Either<A, B> x, Either<A, B> y) => !Equals(x, y);

        internal Either(bool isLeft, A left, B right)
        {
            IsLeft = isLeft;
            Left = left;
            Right = right;
        }

        internal bool IsLeft { get; }
        public bool IsRight => !IsLeft;
        public A Left { get; }
        public B Right { get; }

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

        public override string ToString() => IsLeft ? $"Left({Left})" : $"Right({Right})";
        public override int GetHashCode() => IsLeft ? Left.GetHashCode() : Right.GetHashCode();

        public override bool Equals(object other)
        {
            if (!(other is Either<A, B>))
                return false;

            var that = (Either<A, B>) other;
            return (IsLeft && that.IsLeft && Equals(Left, that.Left))
                || (IsRight && that.IsRight && Equals(Right, that.Right));
        }
    }
}
