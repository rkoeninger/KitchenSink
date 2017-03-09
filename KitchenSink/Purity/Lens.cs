using System;
using System.Linq.Expressions;
using System.Reflection;
using static KitchenSink.Operators;

namespace KitchenSink.Purity
{
    /// <summary>
    /// A composable pair of pure getter/setter functions.
    /// </summary>
    public static class Lens
    {
        // TODO: this mutates the object instead of creating a new one
        //       need to generate call to constructor

        /// <summary>
        /// Builds a Lens for the given property.
        /// </summary>
        public static Lens<A, B> For<A, B>(Expression<Func<A, B>> getExpr)
        {
            if (IsNot<MemberExpression>(getExpr.Body))
            {
                throw new ArgumentException("Expression must be a property");
            }

            var memberExpr = (MemberExpression)getExpr.Body;

            if (IsNot<PropertyInfo>(memberExpr.Member))
            {
                throw new ArgumentException("Expression must be a property");
            }

            var property = (PropertyInfo)memberExpr.Member;

            if (!property.CanWrite)
            {
                throw new ArgumentException($"Property {property.Name} must be writeable.");
            }

            Func<A, B, A> setter = (obj, val) =>
            {
                property.SetValue(obj, val);
                return obj;
            };
            return new Lens<A, B>(getExpr.Compile(), setter);
        }
    }

    /// <summary>
    /// A composable pair of pure getter/setter functions.
    /// </summary>
    public sealed class Lens<A, B>
    {
        public Lens(Func<A, B> get, Func<A, B, A> set)
        {
            Get = get;
            Set = set;
        }

        public Func<A, B> Get { get; }
        public Func<A, B, A> Set { get; }

        public Lens<A, C> Compose<C>(Lens<B, C> other)
        {
            var me = this;
            return new Lens<A, C>(
                a => other.Get(me.Get(a)),
                (a, c) => me.Set(a, other.Set(me.Get(a), c)));
        }
    }
}
