using System;
using System.Collections.Generic;
using System.Linq;
using KitchenSink.Extensions;
using static KitchenSink.Operators;

namespace KitchenSink
{
    internal class MultiMethod
    {
        // Throws InvalidOperationException if child is not descendant of parent
        internal static int DegreesOfSeparation(Type child, Type parent) =>
            child == parent
                ? 0
                : child.GetInterfaces()
                    .Where(t => t.IsAssignableTo(parent))
                    .Min(t => DegreesOfSeparation(t, parent));

        // Throws InvalidOperationException if child is not descendant of parent
        internal static int HierarchyDistance(Type child, Type parent)
        {
            var count = 0;

            while (child != parent)
            {
                child = child.BaseType;
                count++;
            }

            return count;
        }

        internal static Maybe<Type> FindBase(Type t, Type t0, Type t1)
        {
            for (;; t = t.BaseType)
            {
                if (t == t0) return Some(t0);
                if (t == t1) return Some(t1);
                if (t == typeof(object)) return None<Type>();
            }
        }

        internal static Func<Type, Type, Type> NearestMatch(Type t) => (t0, t1) =>
        {
            // Exact match is closer
            if (t == t0) return t0;
            if (t == t1) return t1;

            if (t.IsInterface)
            {
                return DegreesOfSeparation(t, t0) <= DegreesOfSeparation(t, t1) ? t0 : t1;
            }
            else
            {
                var d0 = HierarchyDistance(t, t0);
                var d1 = HierarchyDistance(t, t1);
                return d0 == int.MaxValue && d1 == int.MaxValue
                    ? DegreesOfSeparation(t, t0) <= DegreesOfSeparation(t, t1) ? t0 : t1
                    : d0 <= d1 ? t0 : t1;
            }
        };

        internal static Maybe<Type> NearestMatch(object x, IEnumerable<Type> ts) =>
            x == null
                ? ts.FirstMaybe(t => t == typeof(Void))
                : ts.Where(x.GetType().IsAssignableTo)
                    .AggregateMaybe(NearestMatch(x.GetType()));

        internal static bool VoidMatch(Type t, object x) =>
            x == null && t == typeof(Void);

        internal static bool SubtypeMatch(Type t, object x) =>
            VoidMatch(t, x) || t.IsAssignableFrom(TypeOf(x));

        internal static bool ExactTypeMatch(Type t, object x) =>
            VoidMatch(t, x) || t == TypeOf(x);
    }

    public class MultiMethod<A, Z>
    {
        private readonly List<Func<A, Maybe<Z>>> methods = new List<Func<A, Maybe<Z>>>();

        public Maybe<Z> ApplyMaybe(A a) => methods.FirstSome(m => m(a));

        public Z Apply(A a) => ApplyMaybe(a).OrElseThrow<NotImplementedException>();

        public MultiMethod<A, Z> Extend(Func<A, Maybe<Z>> f)
        {
            methods.Add(f);
            return this;
        }

        public MultiMethod<A, Z> Extend(Func<A, bool> p, Func<A, Z> f) =>
            Extend(a => Maybe.If(p(a), () => f(a)));

        public MultiMethod<A, Z> Extend<A2>(Func<A2, Z> f)
            where A2 : A =>
            Extend(a =>
                Maybe.If(
                    MultiMethod.SubtypeMatch(typeof(A2), a),
                    () => f((A2) a)));

        public MultiMethod<A, Z> ExtendExact<A2>(Func<A2, Z> f)
            where A2 : A =>
            Extend(a =>
                Maybe.If(
                    MultiMethod.ExactTypeMatch(typeof(A2), a),
                    () => f((A2)a)));
    }

    public class MultiMethod<A, B, Z>
    {
        private readonly List<Func<A, B, Maybe<Z>>> methods = new List<Func<A, B, Maybe<Z>>>();

        public Maybe<Z> ApplyMaybe(A a, B b) => methods.FirstSome(m => m(a, b));

        public Z Apply(A a, B b) => ApplyMaybe(a, b).OrElseThrow<NotImplementedException>();

        public MultiMethod<A, B, Z> Extend(Func<A, B, Maybe<Z>> f)
        {
            methods.Add(f);
            return this;
        }

        public MultiMethod<A, B, Z> Extend(Func<A, B, bool> p, Func<A, B, Z> f) =>
            Extend((a, b) => Maybe.If(p(a, b), () => f(a, b)));

        public MultiMethod<A, B, Z> Extend(Func<A, bool> pa, Func<B, bool> pb, Func<A, B, Z> f) =>
            Extend((a, b) => Maybe.If(pa(a) && pb(b), () => f(a, b)));

        public MultiMethod<A, B, Z> Extend<A2, B2>(Func<A2, B2, Z> f)
            where A2 : A
            where B2 : B =>
            Extend((a, b) =>
                Maybe.If(
                    MultiMethod.SubtypeMatch(typeof(A2), a)
                    && MultiMethod.SubtypeMatch(typeof(B2), b),
                    () => f((A2) a, (B2) b)));

        public MultiMethod<A, B, Z> ExtendExact<A2, B2>(Func<A2, B2, Z> f)
            where A2 : A
            where B2 : B =>
            Extend((a, b) =>
                Maybe.If(
                    MultiMethod.ExactTypeMatch(typeof(A2), a)
                    && MultiMethod.ExactTypeMatch(typeof(B2), b),
                    () => f((A2) a, (B2) b)));
    }

    public class MultiMethod<A, B, C, Z>
    {
        private readonly List<Func<A, B, C, Maybe<Z>>> methods = new List<Func<A, B, C, Maybe<Z>>>();

        public Maybe<Z> ApplyMaybe(A a, B b, C c) => methods.FirstSome(m => m(a, b, c));

        public Z Apply(A a, B b, C c) => ApplyMaybe(a, b, c).OrElseThrow<NotImplementedException>();

        public MultiMethod<A, B, C, Z> Extend(Func<A, B, C, Maybe<Z>> f)
        {
            methods.Add(f);
            return this;
        }

        public MultiMethod<A, B, C, Z> Extend(Func<A, B, C, bool> p, Func<A, B, C, Z> f) =>
            Extend((a, b, c) => Maybe.If(p(a, b, c), () => f(a, b, c)));

        public MultiMethod<A, B, C, Z> Extend(Func<A, bool> pa, Func<B, bool> pb, Func<C, bool> pc, Func<A, B, C, Z> f) =>
            Extend((a, b, c) => Maybe.If(pa(a) && pb(b) && pc(c), () => f(a, b, c)));

        public MultiMethod<A, B, C, Z> Extend<A2, B2, C2>(Func<A2, B2, C2, Z> f)
            where A2 : A
            where B2 : B
            where C2 : C =>
            Extend((a, b, c) =>
                Maybe.If(
                    MultiMethod.SubtypeMatch(typeof(A2), a)
                    && MultiMethod.SubtypeMatch(typeof(B2), b)
                    && MultiMethod.SubtypeMatch(typeof(C2), c),
                    () => f((A2) a, (B2) b, (C2) c)));

        public MultiMethod<A, B, C, Z> ExtendExact<A2, B2, C2>(Func<A2, B2, C2, Z> f)
            where A2 : A
            where B2 : B
            where C2 : C =>
            Extend((a, b, c) =>
                Maybe.If(
                    MultiMethod.ExactTypeMatch(typeof(A2), a)
                    && MultiMethod.ExactTypeMatch(typeof(B2), b)
                    && MultiMethod.ExactTypeMatch(typeof(C2), c),
                    () => f((A2) a, (B2) b, (C2) c)));
    }
}
