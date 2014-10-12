namespace ZedSharp
{
    /// <summary>
    /// Unit contains only one value that is used as a placeholder.
    /// A meaningfully different instance of Unit cannot be created.
    /// All references to Unit will be equal.
    /// </summary>
    public struct Unit
    {
        public static readonly Unit It = new Unit();

        public override int GetHashCode()
        {
            return 1;
        }

        public override bool Equals(object obj)
        {
            return obj is Unit;
        }

        public static bool operator ==(Unit u1, Unit u2)
        {
            return true;
        }

        public static bool operator !=(Unit u1, Unit u2)
        {
            return false;
        }

        public override string ToString()
        {
            return "Unit";
        }
    }
}
