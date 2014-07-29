namespace ZedSharp
{
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
            return ! (u1 == u2);
        }

        public override string ToString()
        {
            return "Unit";
        }
    }
}
