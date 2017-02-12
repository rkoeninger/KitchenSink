using System;

namespace KitchenSink
{
    public static class HList
    {
        public static HList<T, Void> Singleton<T>(T x)
        {
            return new HList<T, Void>(x, Void.It);
        }
    }

    public class HList<A, B> : IHList<A, B>
    {
        public A First { get; }
        public B Rest { get; }

        public HList(A first, B rest)
        {
            First = first;
            Rest = rest;
        }

        public IHList<C, IHList<A, B>> Cons<C>(C x)
        {
            return new HList<C, HList<A, B>>(x, this);
        }

        public override string ToString()
        {
            return Rest == null
                ? typeof(A).Name
                : typeof(A).Name + ", " + Rest;
        }
    }

    public interface IHList<out A, out B>
    {
        A First { get; }
        B Rest { get; }
        IHList<C, IHList<A, B>> Cons<C>(C x);
    }

    public static class Tup
    {
        public static Tup<A> Create<A>(A a)
        {
            return new Tup<A>(a);
        }

        public static Tup<A, B> Create<A, B>(A a, B b)
        {
            return new Tup<A, B>(Create(a), b);
        }

        public static Tup<A, B, C> Create<A, B, C>(A a, B b, C c)
        {
            return new Tup<A, B, C>(Create(a, b), c);
        }

        public static Tup<A, B, C, D> Create<A, B, C, D>(A a, B b, C c, D d)
        {
            return new Tup<A, B, C, D>(Create(a, b, c), d);
        }
    }

    public class Tup<A> : IHList<A, Void>
    {
        public A First { get; }

        public Void Rest
        {
            get { return Void.It; }
        }

        public Tup(A x)
        {
            First = x;
        }

        public IHList<B, IHList<A, Void>> Cons<B>(B x)
        {
            return new Tup<A, B>(this, x);
        }

        public override string ToString()
        {
            return Rest == null
                ? typeof(A).Name
                : typeof(A).Name + ", " + Rest;
        }
    }

    public class Tup<A, B> : IHList<B, IHList<A, Void>>
    {
        public B First { get; }
        public IHList<A, Void> Rest { get; }

        public Tup(Tup<A> x, B y)
        {
            First = y;
            Rest = x;
        }

        public IHList<C, IHList<B, IHList<A, Void>>> Cons<C>(C x)
        {
            return new Tup<A, B, C>(this, x);
        }

        public override string ToString()
        {
            return Rest == null
                ? typeof(B).Name
                : typeof(B).Name + ", " + Rest;
        }
    }

    public class Tup<A, B, C> : IHList<C, IHList<B, IHList<A, Void>>>
    {
        public C First { get; }
        public IHList<B, IHList<A, Void>> Rest { get; }

        public Tup(Tup<A, B> x, C y)
        {
            First = y;
            Rest = x;
        }

        public IHList<D, IHList<C, IHList<B, IHList<A, Void>>>> Cons<D>(D x)
        {
            return new Tup<A, B, C, D>(this, x);
        }

        public override string ToString()
        {
            return Rest == null
                ? typeof(C).Name
                : typeof(C).Name + ", " + Rest;
        }
    }

    public class Tup<A, B, C, D> : IHList<D, IHList<C, IHList<B, IHList<A, Void>>>>
    {
        public D First { get; }
        public IHList<C, IHList<B, IHList<A, Void>>> Rest { get; }

        public Tup(Tup<A, B, C> x, D y)
        {
            First = y;
            Rest = x;
        }

        public IHList<E, IHList<D, IHList<C, IHList<B, IHList<A, Void>>>>> Cons<E>(E x)
        {
            throw new Exception("tuples greater than arity 4 not implemented");
        }

        public override string ToString()
        {
            return Rest == null
                ? typeof(D).Name
                : typeof(D).Name + ", " + Rest;
        }
    }
}
