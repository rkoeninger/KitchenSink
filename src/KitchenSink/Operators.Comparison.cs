using System;
using KitchenSink.Collections;
using KitchenSink.Control;

namespace KitchenSink
{
    public static partial class Operators
    {
        /// <summary>
        /// Curried lower bound comparison.
        /// </summary>
        public static Func<A, bool> LowerBound<A>(A x) where A : IComparable<A> =>
            y => y.CompareTo(x) >= 0;

        /// <summary>
        /// Curried upper bound comparision.
        /// </summary>
        public static Func<A, bool> UpperBound<A>(A x) where A : IComparable<A> =>
            y => y.CompareTo(x) <= 0;

        /// <summary>
        /// Curried exclusive lower bound comparision.
        /// </summary>
        public static Func<A, bool> ExclusiveLowerBound<A>(A x) where A : IComparable<A> =>
            y => y.CompareTo(x) > 0;

        /// <summary>
        /// Curried exclusive upper bound comparision.
        /// </summary>
        public static Func<A, bool> ExclusiveUpperBound<A>(A x) where A : IComparable<A> =>
            y => y.CompareTo(x) < 0;

        /// <summary>
        /// Starting point for a range comparison, example: <c>0 &lt;= Cmp(x) &lt; 10</c>
        /// </summary>
        public static RangeComparison.Initial<A> Cmp<A>(A value) where A : IComparable<A> =>
            RangeComparison.New(value);

        /// <summary>
        /// Comparison that returns a symbolic result.
        /// Null values are always less than non-null values.
        /// </summary>
        public static Ordering Compare<A>(A x, A y) where A : IComparable<A> =>
            If(Null(x) && Null(y)).Then(Ordering.Eq)
            .If(Null(x)).Then(Ordering.Lt)
            .If(Null(y)).Then(Ordering.Gt)
            .Else(() =>
                Switch(x?.CompareTo(y) ?? 0)
                .When(Neg).Then(Ordering.Lt)
                .When(Pos).Then(Ordering.Gt)
                .Else(Ordering.Eq));

        public static class RangeComparison
        {
            internal static Initial<TValue> New<TValue>(TValue value) where TValue : IComparable<TValue> =>
                new Initial<TValue>(value);

            internal enum Op
            {
                LessThan,
                LessThanEqual,
                GreaterThan,
                GreaterThanEqual
            }

            public struct Initial<TValue> where TValue : IComparable<TValue>
            {
                private readonly TValue value;

                internal Initial(TValue value) : this() => this.value = value;

                public static Left<TValue> operator <(TValue leftValue, Initial<TValue> builder) =>
                    new Left<TValue>(builder.value, Op.LessThan, leftValue);

                public static Left<TValue> operator >(TValue leftValue, Initial<TValue> builder) =>
                    new Left<TValue>(builder.value, Op.GreaterThan, leftValue);

                public static Left<TValue> operator <=(TValue leftValue, Initial<TValue> builder) =>
                    new Left<TValue>(builder.value, Op.LessThanEqual, leftValue);

                public static Left<TValue> operator >=(TValue leftValue, Initial<TValue> builder) =>
                    new Left<TValue>(builder.value, Op.GreaterThanEqual, leftValue);
            }

            public struct Left<TValue> where TValue : IComparable<TValue>
            {
                private readonly TValue value;
                private readonly Op leftOp;
                private readonly TValue leftValue;

                internal Left(TValue value, Op leftOp, TValue leftValue) : this()
                {
                    this.value = value;
                    this.leftOp = leftOp;
                    this.leftValue = leftValue;
                }

                public static bool operator <(Left<TValue> builder, TValue rightValue) =>
                    DoCompare(builder.leftValue, builder.leftOp, builder.value, Op.LessThan, rightValue);

                public static bool operator >(Left<TValue> builder, TValue rightValue) =>
                    DoCompare(builder.leftValue, builder.leftOp, builder.value, Op.GreaterThan, rightValue);

                public static bool operator <=(Left<TValue> builder, TValue rightValue) =>
                    DoCompare(builder.leftValue, builder.leftOp, builder.value, Op.LessThanEqual, rightValue);

                public static bool operator >=(Left<TValue> builder, TValue rightValue) =>
                    DoCompare(builder.leftValue, builder.leftOp, builder.value, Op.GreaterThanEqual, rightValue);
            }

            private static bool DoCompare<TValue>(
                TValue leftValue,
                Op leftOp,
                TValue value,
                Op rightOp,
                TValue rightValue) where TValue : IComparable<TValue> =>
                DoCompare(leftValue, leftOp, value) && DoCompare(value, rightOp, rightValue);

            private static bool DoCompare<TValue>(TValue left, Op op, TValue right) where TValue : IComparable<TValue>
            {
                var z = left.CompareTo(right);

                return op switch
                {
                    Op.LessThan => (z < 0),
                    Op.LessThanEqual => (z <= 0),
                    Op.GreaterThan => (z > 0),
                    Op.GreaterThanEqual => (z >= 0),
                    _ => throw new ArgumentException("Invalid comparison operator")
                };
            }
        }
    }
}
