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
    }
}
