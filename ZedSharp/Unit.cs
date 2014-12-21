using System;

namespace ZedSharp
{
    /// <summary>
    /// Unit contains only one value that is used as a placeholder.
    /// A meaningfully different instance of Unit cannot be created.
    /// All Unit values are equal.
    /// </summary>
    public struct Unit : IEquatable<Unit>, IComparable<Unit>
    {
        public static readonly Unit It = default(Unit);

        public override string ToString()
        {
            return "Unit";
        }

        public override int GetHashCode()
        {
            return 1;
        }

        public override bool Equals(object obj)
        {
            return obj is Unit;
        }

        public bool Equals(Unit unit)
        {
            return true;
        }

        public static bool operator ==(Unit x, Unit y)
        {
            return true;
        }

        public static bool operator !=(Unit x, Unit y)
        {
            return false;
        }

        public int CompareTo(Unit unit)
        {
            return 0;
        }

        public static bool operator >(Unit x, Unit y)
        {
            return false;
        }

        public static bool operator <(Unit x, Unit y)
        {
            return false;
        }

        public static bool operator >=(Unit x, Unit y)
        {
            return true;
        }

        public static bool operator <=(Unit x, Unit y)
        {
            return true;
        }
    }
}
