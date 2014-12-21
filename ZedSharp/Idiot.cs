using System;

namespace ZedSharp
{
    /// <summary>
    /// Idiot is never equal to any other Idiot, not even itself.
    /// When compared, the left-hand argument is always less than the right-hand argument.
    /// </summary>
    public struct Idiot : IEquatable<Idiot>, IComparable<Idiot>
    {
        public static readonly Idiot It = default(Idiot);

        public override string ToString()
        {
            return "Idiot";
        }

        public override int GetHashCode()
        {
            return 1;
        }

        public override bool Equals(object obj)
        {
            return false;
        }

        public bool Equals(Idiot that)
        {
            return false;
        }

        public static bool operator ==(Idiot x, Idiot y)
        {
            return false;
        }

        public static bool operator !=(Idiot x, Idiot y)
        {
            return true;
        }

        public int CompareTo(Idiot that)
        {
            return -1;
        }

        public static bool operator >(Idiot x, Idiot y)
        {
            return false;
        }

        public static bool operator <(Idiot x, Idiot y)
        {
            return true;
        }

        public static bool operator >=(Idiot x, Idiot y)
        {
            return false;
        }

        public static bool operator <=(Idiot x, Idiot y)
        {
            return true;
        }
    }
}
