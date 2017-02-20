using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace KitchenSink.Collections
{
    public static class ConstructionOperators
    {
        public static Maybe<A> maybeof<A>(A value)
        {
            return Maybe.Of(value);
        }

        public static Maybe<A> some<A>(A value)
        {
            return Maybe.Some(value);
        }

        public static Maybe<A> none<A>()
        {
            return Maybe<A>.None;
        }

        public static IEnumerable<A> seqof<A>(params A[] values)
        {
            return values;
        }

        public static A[] arrayof<A>(params A[] values)
        {
            return values;
        }

        public static List<A> listof<A>(params A[] values)
        {
            return new List<A>(values);
        }

        public static HashSet<A> setof<A>(params A[] values)
        {
            return new HashSet<A>(values);
        }

        public static ConcurrentBag<A> bagof<A>(params A[] values)
        {
            return new ConcurrentBag<A>(values);
        }

        public static Queue<A> queueof<A>(params A[] values)
        {
            return new Queue<A>(values);
        }

        public static Tuple<A, B> tupleof<A, B>(A a, B b)
        {
            return Tuple.Create(a, b);
        }

        public static Tuple<A, B, C> tupleof<A, B, C>(A a, B b, C c)
        {
            return Tuple.Create(a, b, c);
        }

        public static Tuple<A, B, C, D> tupleof<A, B, C, D>(A a, B b, C c, D d)
        {
            return Tuple.Create(a, b, c, d);
        }

        public static Tuple<A, B, C, D, E> tupleof<A, B, C, D, E>(A a, B b, C c, D d, E e)
        {
            return Tuple.Create(a, b, c, d, e);
        }

        public static Tuple<A, B, C, D, E, F> tupleof<A, B, C, D, E, F>(A a, B b, C c, D d, E e, F f)
        {
            return Tuple.Create(a, b, c, d, e, f);
        }

        public static Tuple<A, B, C, D, E, F, G> tupleof<A, B, C, D, E, F, G>(A a, B b, C c, D d, E e, F f, G g)
        {
            return Tuple.Create(a, b, c, d, e, f, g);
        }

        public static Dictionary<A, V> dictof<A, V>()
        {
            return new Dictionary<A, V>();
        }

        public static Dictionary<A, V> dictof<A, V>(
            A k0, V v0)
        {
            return new Dictionary<A, V>
            {
                {k0, v0}
            };
        }

        public static Dictionary<A, V> dictof<A, V>(
            A k0, V v0,
            A k1, V v1)
        {
            return new Dictionary<A, V>
            {
                {k0, v0},
                {k1, v1}
            };
        }

        public static Dictionary<A, V> dictof<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2)
        {
            return new Dictionary<A, V>
            {
                {k0, v0},
                {k1, v1},
                {k2, v2}
            };
        }

        public static Dictionary<A, V> dictof<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3)
        {
            return new Dictionary<A, V>
            {
                {k0, v0},
                {k1, v1},
                {k2, v2},
                {k3, v3}
            };
        }

        public static Dictionary<A, V> dictof<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4)
        {
            return new Dictionary<A, V>
            {
                {k0, v0},
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4}
            };
        }

        public static Dictionary<A, V> dictof<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5)
        {
            return new Dictionary<A, V>
            {
                {k0, v0},
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4},
                {k5, v5}
            };
        }

        public static Dictionary<A, V> dictof<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6)
        {
            return new Dictionary<A, V>
            {
                {k0, v0},
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4},
                {k5, v5},
                {k6, v6}
            };
        }

        public static Dictionary<A, V> dictof<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6,
            A k7, V v7)
        {
            return new Dictionary<A, V>
            {
                {k0, v0},
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4},
                {k5, v5},
                {k6, v6},
                {k7, v7}
            };
        }

        public static Dictionary<A, V> dictof<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6,
            A k7, V v7,
            A k8, V v8)
        {
            return new Dictionary<A, V>
            {
                {k0, v0},
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4},
                {k5, v5},
                {k6, v6},
                {k7, v7},
                {k8, v8}
            };
        }

        public static Dictionary<A, V> dictof<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6,
            A k7, V v7,
            A k8, V v8,
            A k9, V v9)
        {
            return new Dictionary<A, V>
            {
                {k0, v0},
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4},
                {k5, v5},
                {k6, v6},
                {k7, v7},
                {k8, v8},
                {k9, v9}
            };
        }

        public static Dictionary<A, V> dictof<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6,
            A k7, V v7,
            A k8, V v8,
            A k9, V v9,
            A k10, V v10)
        {
            return new Dictionary<A, V>
            {
                {k0, v0},
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4},
                {k5, v5},
                {k6, v6},
                {k7, v7},
                {k8, v8},
                {k9, v9},
                {k10, v10}
            };
        }

        public static Dictionary<A, V> dictof<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6,
            A k7, V v7,
            A k8, V v8,
            A k9, V v9,
            A k10, V v10,
            A k11, V v11)
        {
            return new Dictionary<A, V>
            {
                {k0, v0},
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4},
                {k5, v5},
                {k6, v6},
                {k7, v7},
                {k8, v8},
                {k9, v9},
                {k10, v10},
                {k11, v11}
            };
        }

        public static Dictionary<A, V> dictof<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6,
            A k7, V v7,
            A k8, V v8,
            A k9, V v9,
            A k10, V v10,
            A k11, V v11,
            A k12, V v12)
        {
            return new Dictionary<A, V>
            {
                {k0, v0},
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4},
                {k5, v5},
                {k6, v6},
                {k7, v7},
                {k8, v8},
                {k9, v9},
                {k10, v10},
                {k11, v11},
                {k12, v12}
            };
        }

        public static Dictionary<A, V> dictof<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6,
            A k7, V v7,
            A k8, V v8,
            A k9, V v9,
            A k10, V v10,
            A k11, V v11,
            A k12, V v12,
            A k13, V v13)
        {
            return new Dictionary<A, V>
            {
                {k0, v0},
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4},
                {k5, v5},
                {k6, v6},
                {k7, v7},
                {k8, v8},
                {k9, v9},
                {k10, v10},
                {k11, v11},
                {k12, v12},
                {k13, v13}
            };
        }

        public static Dictionary<A, V> dictof<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6,
            A k7, V v7,
            A k8, V v8,
            A k9, V v9,
            A k10, V v10,
            A k11, V v11,
            A k12, V v12,
            A k13, V v13,
            A k14, V v14)
        {
            return new Dictionary<A, V>
            {
                {k0, v0},
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4},
                {k5, v5},
                {k6, v6},
                {k7, v7},
                {k8, v8},
                {k9, v9},
                {k10, v10},
                {k11, v11},
                {k12, v12},
                {k13, v13},
                {k14, v14}
            };
        }

        public static Dictionary<A, V> dictof<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6,
            A k7, V v7,
            A k8, V v8,
            A k9, V v9,
            A k10, V v10,
            A k11, V v11,
            A k12, V v12,
            A k13, V v13,
            A k14, V v14,
            A k15, V v15)
        {
            return new Dictionary<A, V>
            {
                {k0, v0},
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4},
                {k5, v5},
                {k6, v6},
                {k7, v7},
                {k8, v8},
                {k9, v9},
                {k10, v10},
                {k11, v11},
                {k12, v12},
                {k13, v13},
                {k14, v14},
                {k15, v15}
            };
        }

        public static Dictionary<A, V> dictof<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6,
            A k7, V v7,
            A k8, V v8,
            A k9, V v9,
            A k10, V v10,
            A k11, V v11,
            A k12, V v12,
            A k13, V v13,
            A k14, V v14,
            A k15, V v15,
            A k16, V v16)
        {
            return new Dictionary<A, V>
            {
                {k0, v0},
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4},
                {k5, v5},
                {k6, v6},
                {k7, v7},
                {k8, v8},
                {k9, v9},
                {k10, v10},
                {k11, v11},
                {k12, v12},
                {k13, v13},
                {k14, v14},
                {k15, v15},
                {k16, v16}
            };
        }

        public static Dictionary<A, V> dictof<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6,
            A k7, V v7,
            A k8, V v8,
            A k9, V v9,
            A k10, V v10,
            A k11, V v11,
            A k12, V v12,
            A k13, V v13,
            A k14, V v14,
            A k15, V v15,
            A k16, V v16,
            A k17, V v17)
        {
            return new Dictionary<A, V>
            {
                {k0, v0},
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4},
                {k5, v5},
                {k6, v6},
                {k7, v7},
                {k8, v8},
                {k9, v9},
                {k10, v10},
                {k11, v11},
                {k12, v12},
                {k13, v13},
                {k14, v14},
                {k15, v15},
                {k16, v16},
                {k17, v17}
            };
        }

        public static Dictionary<A, V> dictof<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6,
            A k7, V v7,
            A k8, V v8,
            A k9, V v9,
            A k10, V v10,
            A k11, V v11,
            A k12, V v12,
            A k13, V v13,
            A k14, V v14,
            A k15, V v15,
            A k16, V v16,
            A k17, V v17,
            A k18, V v18)
        {
            return new Dictionary<A, V>
            {
                {k0, v0},
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4},
                {k5, v5},
                {k6, v6},
                {k7, v7},
                {k8, v8},
                {k9, v9},
                {k10, v10},
                {k11, v11},
                {k12, v12},
                {k13, v13},
                {k14, v14},
                {k15, v15},
                {k16, v16},
                {k17, v17},
                {k18, v18}
            };
        }

        public static Dictionary<A, V> dictof<A, V>(
            A k0, V v0,
            A k1, V v1,
            A k2, V v2,
            A k3, V v3,
            A k4, V v4,
            A k5, V v5,
            A k6, V v6,
            A k7, V v7,
            A k8, V v8,
            A k9, V v9,
            A k10, V v10,
            A k11, V v11,
            A k12, V v12,
            A k13, V v13,
            A k14, V v14,
            A k15, V v15,
            A k16, V v16,
            A k17, V v17,
            A k18, V v18,
            A k19, V v19)
        {
            return new Dictionary<A, V>
            {
                {k0, v0},
                {k1, v1},
                {k2, v2},
                {k3, v3},
                {k4, v4},
                {k5, v5},
                {k6, v6},
                {k7, v7},
                {k8, v8},
                {k9, v9},
                {k10, v10},
                {k11, v11},
                {k12, v12},
                {k13, v13},
                {k14, v14},
                {k15, v15},
                {k16, v16},
                {k17, v17},
                {k18, v18},
                {k19, v19}
            };
        }
    }
}
