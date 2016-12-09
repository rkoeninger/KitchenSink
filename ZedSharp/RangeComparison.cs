using System;

namespace ZedSharp
{
    public enum RangeComparisonOp
    {
        Lt, LtEq, Gt, GtEq
    }

    public struct RangeComparison0
    {
        public static RangeComparison1 operator -(RangeComparison0 r, IComparable firstBound)
        {
            return new RangeComparison1(firstBound);
        }
    }

    public struct RangeComparison1
    {
        public IComparable FirstBound { get; private set; }

        public RangeComparison1(IComparable firstBound) : this()
        {
            FirstBound = firstBound;
        }

        public static RangeComparison2 operator <(RangeComparison1 r, IComparable value)
        {
            return new RangeComparison2(r.FirstBound, RangeComparisonOp.Lt, value);
        }

        public static RangeComparison2 operator <=(RangeComparison1 r, IComparable value)
        {
            return new RangeComparison2(r.FirstBound, RangeComparisonOp.LtEq, value);
        }

        public static RangeComparison2 operator >(RangeComparison1 r, IComparable value)
        {
            return new RangeComparison2(r.FirstBound, RangeComparisonOp.Gt, value);
        }

        public static RangeComparison2 operator >=(RangeComparison1 r, IComparable value)
        {
            return new RangeComparison2(r.FirstBound, RangeComparisonOp.GtEq, value);
        }
    }

    public struct RangeComparison2
    {
        public IComparable FirstBound { get; private set; }
        public RangeComparisonOp FirstOp { get; private set; }
        public IComparable Value { get; private set; }

        public RangeComparison2(IComparable firstBound, RangeComparisonOp firstOp, IComparable value) : this()
        {
            FirstBound = firstBound;
            FirstOp = firstOp;
            Value = value;
        }

        public static bool operator <(RangeComparison2 r, IComparable secondBound)
        {
            return RangeComparisons.Compare(r.FirstBound, r.FirstOp, r.Value, RangeComparisonOp.Lt, secondBound);
        }

        public static bool operator <=(RangeComparison2 r, IComparable secondBound)
        {
            return RangeComparisons.Compare(r.FirstBound, r.FirstOp, r.Value, RangeComparisonOp.LtEq, secondBound);
        }

        public static bool operator >(RangeComparison2 r, IComparable secondBound)
        {
            return RangeComparisons.Compare(r.FirstBound, r.FirstOp, r.Value, RangeComparisonOp.Gt, secondBound);
        }

        public static bool operator >=(RangeComparison2 r, IComparable secondBound)
        {
            return RangeComparisons.Compare(r.FirstBound, r.FirstOp, r.Value, RangeComparisonOp.GtEq, secondBound);
        }
    }

    internal static class RangeComparisons
    {
        public static bool Compare(
            IComparable firstBound,
            RangeComparisonOp firstOp,
            IComparable value,
            RangeComparisonOp secondOp,
            IComparable secondBound)
        {
            return Compare(firstBound, firstOp, value) && Compare(value, secondOp, secondBound);
        }
        
        public static bool Compare(IComparable x, RangeComparisonOp op, IComparable y)
        {
            var z = x.CompareTo(y);

            switch (op)
            {
                case RangeComparisonOp.Lt:   return z < 0;
                case RangeComparisonOp.LtEq: return z <= 0;
                case RangeComparisonOp.Gt:   return z > 0;
                case RangeComparisonOp.GtEq: return z >= 0;
            }

            throw new ArgumentException("Invalid comparison operator");
        }
    }
}
