using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KitchenSink.Extensions;
using static KitchenSink.Operators;

namespace KitchenSink.Testing
{
    public static class Check
    {
        public static void That<A>(
            IEnumerable<A> args,
            Func<A, bool> f) =>
            args.FirstMaybe(Compose(f, Not))
                .ForEach(t => throw new PropertyRefutedException(t));

        public static void That<A, B>(
            IEnumerable<A> aSeq,
            IEnumerable<B> bSeq,
            Func<A, B, bool> f) => (
            from a in aSeq
            from b in bSeq
            select (a, b))
            .FirstMaybe(Compose(Tuplize(f), Not))
                .ForEach(t => throw new PropertyRefutedException(t.AsEnumerable().ToArray()));

        public static void That<A, B, C>(
            IEnumerable<A> aSeq,
            IEnumerable<B> bSeq,
            IEnumerable<C> cSeq,
            Func<A, B, C, bool> f) => (
            from a in aSeq
            from b in bSeq
            from c in cSeq
            select (a, b, c))
            .FirstMaybe(Compose(Tuplize(f), Not))
                .ForEach(t => throw new PropertyRefutedException(t.AsEnumerable().ToArray()));

        public static void That<A, B, C, D>(
            IEnumerable<A> aSeq,
            IEnumerable<B> bSeq,
            IEnumerable<C> cSeq,
            IEnumerable<D> dSeq,
            Func<A, B, C, D, bool> f) => (
            from a in aSeq
            from b in bSeq
            from c in cSeq
            from d in dSeq
            select (a, b, c, d))
            .FirstMaybe(Compose(Tuplize(f), Not))
                .ForEach(t => throw new PropertyRefutedException(t.AsEnumerable().ToArray()));

        private static readonly Dictionary<Type, IEnumerable> DefaultInputs = DictOf<Type, IEnumerable>(
            typeof(int), Sample.Ints,
            typeof(IEnumerable<int>), Rand.Lists(Rand.Ints()));

        public static void That<A>(Func<A, bool> f) =>
            ((IEnumerable<A>) DefaultInputs[typeof(A)]).With(xs => That(xs, f));

        public static void That<A, B>(Func<A, B, bool> f)
        {
            var testData0 = (IEnumerable<A>) DefaultInputs[typeof(A)];
            var testData1 = (IEnumerable<B>) DefaultInputs[typeof(B)];
            That(testData0, testData1, f);
        }

        public static void ReflexiveEquality<A>(IEnumerable<A> data) =>
            data.ToList().With(xs => That(xs, xs, (x, y) => Equals(x, y) == Equals(y, x)));

        public static void ReflexiveEquality<A>() =>
            ((IEnumerable<A>) DefaultInputs[typeof(A)]).With(ReflexiveEquality);

        public static void Comparable<A>(IEnumerable<A> data) where A : IComparable<A> =>
            data.ToList().With(xs => That(xs, xs, (x, y) => x.CompareTo(y) == -(y.CompareTo(x))));

        public static void CompareOperators<A>(IEnumerable<A> data) =>
            data.ToList().With(xs => That(xs, xs, (x, y) =>
            {
                dynamic dx = x;
                dynamic dy = y;
                return dx > dy == !(dx <= dy) && dx < dy == !(dx >= dy);
            }));

        public static void EqualsAndHashCode<A>(IEnumerable<A> data) =>
            data.ToList().With(xs => That(xs, xs, (x, y) => Implies(Equals(x, y), Hash(x) == Hash(y))));

        public static void Idempotent<A>(Func<A, A> f) =>
            Idempotent((IEnumerable<A>) DefaultInputs[typeof(A)], f);

        public static void Idempotent<A>(IEnumerable<A> data, Func<A, A> f) =>
            That(data, x => Equals(f(x), f(f(x))));
    }
}
