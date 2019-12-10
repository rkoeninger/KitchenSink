using System;

namespace KitchenSink.Collections
{
    /// <summary>
    /// Classic Lisp cons.
    /// </summary>
    public class Cons : IEquatable<Cons>
    {
        public Cons(object car, object cdr)
        {
            Car = car;
            Cdr = cdr;
        }

        public object Car { get; }
        public object Cdr { get; }

        public static bool operator ==(Cons x, Cons y) => Equals(x, y);
        public static bool operator !=(Cons x, Cons y) => !Equals(x, y);

        public override bool Equals(object that) => that is Cons c && Equals(c);

        public bool Equals(Cons c) => c != null && Equals(Car, c.Car) && Equals(Cdr, c.Cdr);

        public override int GetHashCode() => (Car?.GetHashCode() ?? 0) ^ (Cdr?.GetHashCode() ?? 0);

        public override string ToString() => $"({Car} . {Cdr})";
    }
}
