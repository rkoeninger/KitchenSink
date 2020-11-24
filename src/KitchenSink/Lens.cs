using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using KitchenSink.Extensions;

namespace KitchenSink
{
    /// <summary>
    /// A composable pair of pure getter/setter functions.
    /// </summary>
    public static class Lens
    {
        public static Lens<A, B> Of<A, B>(Func<A, B> get, Func<A, B, A> set) =>
            new Lens<A, B>(get, set);

        /// <summary>
        /// Builds a Lens by reflectively lookuping property referenced in given lambda.
        /// </summary>
        public static Lens<A, B> Of<A, B>(Expression<Func<A, B>> getExpr) =>
            new Lens<A, B>(getExpr.Compile(), Setter(getExpr));

        private static Func<A, B, A> Setter<A, B>(Expression<Func<A, B>> getExpr)
        {
            var name = getExpr.GetProperty().Name;
            var ctor = typeof(A)
                .GetConstructors()
                .SingleOrDefault(c => c.GetParameters().Length > 0)
                ?? throw new InvalidOperationException(
                    $"Type {typeof(A)} has more than one constructor");
            var properties = typeof(A).GetProperties();
            var paramz = ctor.GetParameters();
            return (record, value) =>
                (A) ctor
                    .Invoke(paramz
                        .Select(p => p.Name.IsSimilar(name)
                            ? value
                            : Get<A>(record, properties, p))
                        .ToArray());
        }

        private static object Get<A>(object target, IEnumerable<PropertyInfo> properties, ParameterInfo param)
        {
            var property = properties.FirstOrDefault(x => x.Name.IsSimilar(param.Name))
                ?? throw new InvalidOperationException(
                    $"Constructor for type {typeof(A)} has parameters that do not match properties");
            return property.GetValue(target, null);
        }
    }

    /// <summary>
    /// A composable pair of pure getter/setter functions.
    /// </summary>
    public sealed class Lens<A, B>
    {
        public Lens(Func<A, B> get, Func<A, B, A> set) => (Get, Set) = (get, set);

        public Func<A, B> Get { get; }
        public Func<A, B, A> Set { get; }

        public Lens<C, B> Compose<C>(Lens<C, A> other) => other.Then(this);

        public Lens<A, C> Then<C>(Lens<B, C> other) =>
            new Lens<A, C>(
                a => other.Get(Get(a)),
                (a, c) => Set(a, other.Set(Get(a), c)));
    }
}
