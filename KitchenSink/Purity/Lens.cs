using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using KitchenSink.Extensions;
using static KitchenSink.Operators;

namespace KitchenSink.Purity
{
    /// <summary>
    /// A composable pair of pure getter/setter functions.
    /// </summary>
    public static class Lens
    {
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
            return new Lens<A, B>(getExpr.Compile(), Setter<A, B>(property.Name));
        }

        private static Func<A, B, A> Setter<A, B>(string name)
        {
            var ctor = typeof(A)
                .GetConstructors()
                .SingleOrDefault(c => c.GetParameters().Length > 0);

            if (ctor == null)
            {
                throw new InvalidOperationException(
                    $"Type {typeof(A)} has more than one constructor");
            }

            var properties = typeof(A).GetProperties();
            var paramz = ctor.GetParameters();
            return (record, value) =>
            {
                var args = paramz
                    .Select(p => p.Name.IsSimilar(name)
                        ? value
                        : Get<A>(record, properties, p))
                    .ToArray();
                return (A) ctor.Invoke(args);
            };
        }

        private static object Get<A>(object target, IEnumerable<PropertyInfo> properties, ParameterInfo param)
        {
            var property = properties.FirstOrDefault(x => x.Name.IsSimilar(param.Name));

            if (property == null)
            {
                throw new InvalidOperationException(
                    $"Constructor for type {typeof(A)} has parameters that do not match properties");
            }

            return property.GetValue(target, null);
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
