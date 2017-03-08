using System;

namespace KitchenSink
{
    public enum Comparison
    {
        GT, LT, EQ
    }

    public static class RangeComparison
    {
        internal static Initial<TValue> New<TValue>(TValue value) where TValue : IComparable<TValue>
        {
            return new Initial<TValue>(value);
        }

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

            internal Initial(TValue value) : this()
            {
                this.value = value;
            }

            public static Left<TValue> operator <(TValue leftValue, Initial<TValue> builder)
            {
                return new Left<TValue>(builder.value, Op.LessThan, leftValue);
            }

            public static Left<TValue> operator >(TValue leftValue, Initial<TValue> builder)
            {
                return new Left<TValue>(builder.value, Op.GreaterThan, leftValue);
            }

            public static Left<TValue> operator <=(TValue leftValue, Initial<TValue> builder)
            {
                return new Left<TValue>(builder.value, Op.LessThanEqual, leftValue);
            }

            public static Left<TValue> operator >=(TValue leftValue, Initial<TValue> builder)
            {
                return new Left<TValue>(builder.value, Op.GreaterThanEqual, leftValue);
            }
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

            public static bool operator <(Left<TValue> builder, TValue rightValue)
            {
                return DoCompare(builder.leftValue, builder.leftOp, builder.value, Op.LessThan, rightValue);
            }

            public static bool operator >(Left<TValue> builder, TValue rightValue)
            {
                return DoCompare(builder.leftValue, builder.leftOp, builder.value, Op.GreaterThan, rightValue);
            }

            public static bool operator <=(Left<TValue> builder, TValue rightValue)
            {
                return DoCompare(builder.leftValue, builder.leftOp, builder.value, Op.LessThanEqual, rightValue);
            }

            public static bool operator >=(Left<TValue> builder, TValue rightValue)
            {
                return DoCompare(builder.leftValue, builder.leftOp, builder.value, Op.GreaterThanEqual, rightValue);
            }
        }

        private static bool DoCompare<TValue>(
            TValue leftValue,
            Op leftOp,
            TValue value,
            Op rightOp,
            TValue rightValue) where TValue : IComparable<TValue>
        {
            return DoCompare(leftValue, leftOp, value) && DoCompare(value, rightOp, rightValue);
        }

        private static bool DoCompare<TValue>(TValue left, Op op, TValue right) where TValue : IComparable<TValue>
        {
            var z = left.CompareTo(right);

            switch (op)
            {
                case Op.LessThan:         return z < 0;
                case Op.LessThanEqual:    return z <= 0;
                case Op.GreaterThan:      return z > 0;
                case Op.GreaterThanEqual: return z >= 0;
                default: throw new ArgumentException("Invalid comparison operator");
            }
        }
    }
}
