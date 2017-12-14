namespace KitchenSink
{
    public abstract class HList<L> where L : HList<L>
    {
        internal HList() { }
        public abstract HCons<C, L> Cons<C>(C x);
    }

    public sealed class HCons<E, L> : HList<HCons<E, L>> where L : HList<L>
    {
        public HCons(E head, L tail)
        {
            Head = head;
            Tail = tail;
        }

        public override HCons<C, HCons<E, L>> Cons<C>(C x) => new HCons<C, HCons<E, L>>(x, this);
        public E Head { get; }
        public L Tail { get; }
    }

    public sealed class HNil : HList<HNil>
    {
        private HNil() { }
        public static readonly HNil It = new HNil();
        public override HCons<C, HNil> Cons<C>(C x) => new HCons<C, HNil>(x, It);
    }

    public static class HList
    {
        public static TKey Get<
            TKey,
            TMore>(
            TKey key,
            HCons<TKey, TMore> list) where TMore : HList<TMore> =>
            list
            .Head;

        public static TKey Get<
            TKey,
            T0,
            TMore>(
            TKey key,
            HCons<T0,
            HCons<TKey, TMore>> list) where TMore : HList<TMore> =>
            list
            .Tail
            .Head;

        public static TKey Get<
            TKey,
            T0,
            T1,
            TMore>(
            TKey key,
            HCons<T0,
            HCons<T1,
            HCons<TKey, TMore>>> list) where TMore : HList<TMore> =>
            list
            .Tail
            .Tail
            .Head;

        public static TKey Get<
            TKey,
            T0,
            T1,
            T2,
            TMore>(
            TKey key,
            HCons<T0,
            HCons<T1,
            HCons<T2,
            HCons<TKey, TMore>>>> list) where TMore : HList<TMore> =>
            list
            .Tail
            .Tail
            .Tail
            .Head;

        public static TKey Get<
            TKey,
            T0,
            T1,
            T2,
            T3,
            TMore>(
            TKey key,
            HCons<T0,
            HCons<T1,
            HCons<T2,
            HCons<T3,
            HCons<TKey, TMore>>>>> list) where TMore : HList<TMore> =>
            list
            .Tail
            .Tail
            .Tail
            .Tail
            .Head;

        public static TKey Get<
            TKey,
            T0,
            T1,
            T2,
            T3,
            T4,
            TMore>(
            TKey key,
            HCons<T0,
            HCons<T1,
            HCons<T2,
            HCons<T3,
            HCons<T4,
            HCons<TKey, TMore>>>>>> list) where TMore : HList<TMore> =>
            list
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Head;

        public static TKey Get<
            TKey,
            T0,
            T1,
            T2,
            T3,
            T4,
            T5,
            TMore>(
            TKey key,
            HCons<T0,
            HCons<T1,
            HCons<T2,
            HCons<T3,
            HCons<T4,
            HCons<T5,
            HCons<TKey, TMore>>>>>>> list) where TMore : HList<TMore> =>
            list
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Head;

        public static TKey Get<
            TKey,
            T0,
            T1,
            T2,
            T3,
            T4,
            T5,
            T6,
            TMore>(
            TKey key,
            HCons<T0,
            HCons<T1,
            HCons<T2,
            HCons<T3,
            HCons<T4,
            HCons<T5,
            HCons<T6,
            HCons<TKey, TMore>>>>>>>> list) where TMore : HList<TMore> =>
            list
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Head;

        public static TKey Get<
            TKey,
            T0,
            T1,
            T2,
            T3,
            T4,
            T5,
            T6,
            T7,
            TMore>(
            TKey key,
            HCons<T0,
            HCons<T1,
            HCons<T2,
            HCons<T3,
            HCons<T4,
            HCons<T5,
            HCons<T6,
            HCons<T7,
            HCons<TKey, TMore>>>>>>>>> list) where TMore : HList<TMore> =>
            list
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Head;

        public static TKey Get<
            TKey,
            T0,
            T1,
            T2,
            T3,
            T4,
            T5,
            T6,
            T7,
            T8,
            TMore>(
            TKey key,
            HCons<T0,
            HCons<T1,
            HCons<T2,
            HCons<T3,
            HCons<T4,
            HCons<T5,
            HCons<T6,
            HCons<T7,
            HCons<T8,
            HCons<TKey, TMore>>>>>>>>>> list) where TMore : HList<TMore> =>
            list
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Head;

        public static TKey Get<
            TKey,
            T0,
            T1,
            T2,
            T3,
            T4,
            T5,
            T6,
            T7,
            T8,
            T9,
            TMore>(
            TKey key,
            HCons<T0,
            HCons<T1,
            HCons<T2,
            HCons<T3,
            HCons<T4,
            HCons<T5,
            HCons<T6,
            HCons<T7,
            HCons<T8,
            HCons<T9,
            HCons<TKey, TMore>>>>>>>>>>> list) where TMore : HList<TMore> =>
            list
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Head;

        public static TKey Get<
            TKey,
            T0,
            T1,
            T2,
            T3,
            T4,
            T5,
            T6,
            T7,
            T8,
            T9,
            T10,
            TMore>(
            TKey key,
            HCons<T0,
            HCons<T1,
            HCons<T2,
            HCons<T3,
            HCons<T4,
            HCons<T5,
            HCons<T6,
            HCons<T7,
            HCons<T8,
            HCons<T9,
            HCons<T10,
            HCons<TKey, TMore>>>>>>>>>>>> list) where TMore : HList<TMore> =>
            list
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Head;

        public static TKey Get<
            TKey,
            T0,
            T1,
            T2,
            T3,
            T4,
            T5,
            T6,
            T7,
            T8,
            T9,
            T10,
            T11,
            TMore>(
            TKey key,
            HCons<T0,
            HCons<T1,
            HCons<T2,
            HCons<T3,
            HCons<T4,
            HCons<T5,
            HCons<T6,
            HCons<T7,
            HCons<T8,
            HCons<T9,
            HCons<T10,
            HCons<T11,
            HCons<TKey, TMore>>>>>>>>>>>>> list) where TMore : HList<TMore> =>
            list
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Head;

        public static TKey Get<
            TKey,
            T0,
            T1,
            T2,
            T3,
            T4,
            T5,
            T6,
            T7,
            T8,
            T9,
            T10,
            T11,
            T12,
            TMore>(
            TKey key,
            HCons<T0,
            HCons<T1,
            HCons<T2,
            HCons<T3,
            HCons<T4,
            HCons<T5,
            HCons<T6,
            HCons<T7,
            HCons<T8,
            HCons<T9,
            HCons<T10,
            HCons<T11,
            HCons<T12,
            HCons<TKey, TMore>>>>>>>>>>>>>> list) where TMore : HList<TMore> =>
            list
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Head;

        public static TKey Get<
            TKey,
            T0,
            T1,
            T2,
            T3,
            T4,
            T5,
            T6,
            T7,
            T8,
            T9,
            T10,
            T11,
            T12,
            T13,
            TMore>(
            TKey key,
            HCons<T0,
            HCons<T1,
            HCons<T2,
            HCons<T3,
            HCons<T4,
            HCons<T5,
            HCons<T6,
            HCons<T7,
            HCons<T8,
            HCons<T9,
            HCons<T10,
            HCons<T11,
            HCons<T12,
            HCons<T13,
            HCons<TKey, TMore>>>>>>>>>>>>>>> list) where TMore : HList<TMore> =>
            list
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Head;

        public static TKey Get<
            TKey,
            T0,
            T1,
            T2,
            T3,
            T4,
            T5,
            T6,
            T7,
            T8,
            T9,
            T10,
            T11,
            T12,
            T13,
            T14,
            TMore>(
            TKey key,
            HCons<T0,
            HCons<T1,
            HCons<T2,
            HCons<T3,
            HCons<T4,
            HCons<T5,
            HCons<T6,
            HCons<T7,
            HCons<T8,
            HCons<T9,
            HCons<T10,
            HCons<T11,
            HCons<T12,
            HCons<T13,
            HCons<T14,
            HCons<TKey, TMore>>>>>>>>>>>>>>>> list) where TMore : HList<TMore> =>
            list
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Tail
            .Head;
    }
}
