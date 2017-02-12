using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KitchenSink
{
    public static class Check
    {
        public static void That<A>(IEnumerable<A> testData0, Func<A, bool> f)
        {
            foreach (var arg0 in testData0)
                if (! f(arg0))
                    throw new ExpectationFailedException("Property refuted with (" + arg0 + ")");
        }

        public static void That<A, B>(IEnumerable<A> testData0, IEnumerable<B> testData1, Func<A, B, bool> f)
        {
            var list0 = testData0.ToList();
            var list1 = testData1.ToList();

            foreach (var arg0 in list0)
                foreach (var arg1 in list1)
                    if (! f(arg0, arg1))
                        throw new ExpectationFailedException("Property refuted with (" + arg0 + ", " + arg1 + ")");
        }

        public static void That<A, B, C>(IEnumerable<A> testData0, IEnumerable<B> testData1, IEnumerable<C> testData2, Func<A, B, C, bool> f)
        {
            var list0 = testData0.ToList();
            var list1 = testData1.ToList();
            var list2 = testData2.ToList();

            foreach (var arg0 in list0)
                foreach (var arg1 in list1)
                    foreach (var arg2 in list2)
                        if (! f(arg0, arg1, arg2))
                            throw new ExpectationFailedException("Property refuted with (" + arg0 + ", " + arg1 + ", " + arg2 + ")");
        }

        private static readonly Dictionary<Type, IEnumerable> DefaultInputs = Dictionary.Of<Type, IEnumerable>(
            typeof(int), Sample.Ints,
            typeof(IEnumerable<int>), Rand.Lists(Rand.Ints()));

        public static void That<A>(Func<A, bool> f)
        {
            var testData0 = (IEnumerable<A>) DefaultInputs[typeof(A)];
            That(testData0, f);
        }

        public static void That<A, B>(Func<A, B, bool> f)
        {
            var testData0 = (IEnumerable<A>) DefaultInputs[typeof(A)];
            var testData1 = (IEnumerable<B>) DefaultInputs[typeof(B)];
            That(testData0, testData1, f);
        }

        public static void ReflexiveEquality<A>(IEnumerable<A> testData0)
        {
            var list0 = testData0.ToList();
            That(list0, list0, (x, y) => Equals(x, y) == Equals(y, x));
        }

        public static void ReflexiveEquality<A>()
        {
            var testData0 = (IEnumerable<A>) DefaultInputs[typeof(A)];
            ReflexiveEquality(testData0);
        }

        public static void Comparable<A>(IEnumerable<A> testData0) where A : IComparable<A>
        {
            var list0 = testData0.ToList();
            That(list0, list0, (x, y) => x.CompareTo(y) == -(y.CompareTo(x)));
        }

        public static void CompareOperators<A>(IEnumerable<A> testData0)
        {
            var list0 = testData0.ToList();
            That(list0, list0, (x, y) =>
            {
                dynamic dx = x;
                dynamic dy = y;
                return (dx > dy) == !(dx <= dy) && (dx < dy) == !(dx >= dy);
            });
        }

        private static int Hash<A>(A x)
        {
            return x.IsNull() ? 0 : x.GetHashCode();
        }

        public static void EqualsAndHashCode<A>(IEnumerable<A> testData0)
        {
            var list0 = testData0.ToList();
            That(list0, list0, (x, y) => Equals(x, y).Implies(Hash(x) == Hash(y)));
        }

        public static void Idempotent<A>(Func<A, A> f)
        {
            Idempotent((IEnumerable<A>) DefaultInputs[typeof(A)], f);
        }

        public static void Idempotent<A>(IEnumerable<A> testData0, Func<A, A> f)
        {
            That(testData0, x => Equals(f(x), f(f(x))));
        }
    }
}
