using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ZedSharp
{
    public static class Lens
    {
        public static Lens<A, B> Of<A, B>(Func<A, B> get, Func<A, B, A> set)
        {
            return new Lens<A, B>(get, set);
        }

        public static LensBuilder<A> From<A>()
        {
            return new LensBuilder<A>();
        }

        public static Lens<A, B> Gen<A, B>(Expression<Func<A, B>> expr)
        {
            if (! (expr.Body is MemberExpression))
                throw new ArgumentException();

            var mexpr = (MemberExpression) expr.Body;

            if (mexpr.Member.MemberType != MemberTypes.Property)
                throw new ArgumentException();

            var propInfo = (PropertyInfo) mexpr.Member;
            return Gen<A, B>(propInfo.Name);
        }

        public static Lens<A, B> Gen<A, B>(String propertyName)
        {
            var objParam = Expression.Parameter(typeof(A));
            var propExpr = Expression.PropertyOrField(objParam, propertyName);
            var getter = Expression.Lambda<Func<A, B>>(propExpr, objParam).Compile();

            var valParam = Expression.Parameter(typeof(B));
            var ctor = typeof(A).GetConstructors().OrderByDescending(x => x.GetParameters().Count()).FirstOrDefault();

            if (ctor == null)
                throw new ArgumentException("Type " + typeof(A) + " does not have an applicable constructor");

            var props = typeof(A).GetProperties();
            var argExprs = ctor.GetParameters().Select(param =>
            {
                if (String.Equals(param.Name, propertyName, StringComparison.InvariantCultureIgnoreCase))
                    return (Expression) valParam;

                var prop = props.Where(x => param.Name.EqualsIgnoreCase(x.Name)).FirstMaybe().OrElseThrow("No property has the same name as constructor parameter: " + param.Name);
                return (Expression) Expression.PropertyOrField(objParam, prop.Name);
            }).ToArray();
            var newExpr = Expression.New(ctor, argExprs);
            var setter = Expression.Lambda<Func<A, B, A>>(newExpr, objParam, valParam).Compile();

            return new Lens<A, B>(getter, setter);
        }
    }

    public struct Lens<A, B>
    {
        internal Lens(Func<A, B> get, Func<A, B, A> set) : this()
        {
            Get = get;
            Set = set;
        }

        public Func<A, B> Get { get; private set; }
        public Func<A, B, A> Set { get; private set; }

        public Lens<A, C> Compose<C>(Lens<B, C> other)
        {
            var me = this;
            return new Lens<A, C>(
                a => other.Get(me.Get(a)),
                (a, c) => me.Set(a, other.Set(me.Get(a), c)));
        }
    }

    public struct LensBuilder<A>
    {
        public Lens<A, B> Of<B>(Func<A, B> get, Func<A, B, A> set)
        {
            return new Lens<A, B>(get, set);
        }

        public Lens<A, B> Gen<B>(Expression<Func<A, B>> get)
        {
            var mexpr = (MemberExpression) get.Body;
            var propertyName = mexpr.Member.Name;
            return Lens.Gen<A, B>(propertyName);
        }
    }
}
