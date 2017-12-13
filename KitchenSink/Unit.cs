using System;

namespace KitchenSink
{
    /// <summary>
    /// Unit contains only one value that is used as a placeholder.
    /// A meaningfully different instance of Unit cannot be created.
    /// All Unit values are equal.
    /// </summary>
    public struct Unit : IEquatable<Unit>, IComparable<Unit>
    {
        /// <summary>
        /// The singleton instance of Unit.
        /// </summary>
        public static readonly Unit It = default;

        /// <summary>
        /// Returns "Unit".
        /// </summary>
        public override string ToString() => "Unit";

        /// <summary>
        /// Returns 1.
        /// </summary>
        public override int GetHashCode() => 1;

        /// <summary>
        /// Returns true if argument is also Unit.
        /// </summary>
        public override bool Equals(object obj) => obj is Unit;

        /// <summary>
        /// Returns true.
        /// </summary>
        public bool Equals(Unit unit) => true;

        /// <summary>
        /// Returns true.
        /// </summary>
        public static bool operator ==(Unit x, Unit y) => true;

        /// <summary>
        /// Returns false.
        /// </summary>
        public static bool operator !=(Unit x, Unit y) => false;

        /// <summary>
        /// Returns 0 - always equal.
        /// </summary>
        public int CompareTo(Unit unit) => 0;

        /// <summary>
        /// Returns false.
        /// </summary>
        public static bool operator >(Unit x, Unit y) => false;

        /// <summary>
        /// Returns false.
        /// </summary>
        public static bool operator <(Unit x, Unit y) => false;

        /// <summary>
        /// Returns true.
        /// </summary>
        public static bool operator >=(Unit x, Unit y) => true;

        /// <summary>
        /// Returns true.
        /// </summary>
        public static bool operator <=(Unit x, Unit y) => true;
    }
}
