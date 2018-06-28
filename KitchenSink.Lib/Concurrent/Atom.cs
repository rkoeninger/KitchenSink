using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace KitchenSink.Concurrent
{
    public static class Atom
    {
        public static Atom<A> Of<A>(A initial) => new BasicAtom<A>(initial);
        public static Atom<Z> Zip<A, B, Z>(
            Atom<A> atomA,
            Atom<B> atomB,
            Func<A, B, Z> combine,
            Func<Z, (A, B)> split) =>
            new ZippedAtom<A, B, Z>(atomA, atomB, combine, split);
        public static Atom<Z> Zip<A, B, C, Z>(
            Atom<A> atomA,
            Atom<B> atomB,
            Atom<C> atomC,
            Func<A, B, C, Z> combine,
            Func<Z, (A, B, C)> split) =>
            new ZippedAtom<A, B, C, Z>(atomA, atomB, atomC, combine, split);
        public static Atom<Z> Zip<A, B, C, D, Z>(
            Atom<A> atomA,
            Atom<B> atomB,
            Atom<C> atomC,
            Atom<D> atomD,
            Func<A, B, C, D, Z> combine,
            Func<Z, (A, B, C, D)> split) =>
            new ZippedAtom<A, B, C, D, Z>(atomA, atomB, atomC, atomD, combine, split);
    }

    public abstract class Atom<A>
    {
        internal Atom() { }

        internal abstract Lock Lock { get; }

        public abstract A Update(Func<A, A> f);

        public A Value
        {
            get => Update(x => x);
            set => Update(_ => value);
        }

        public Task<A> UpdateAsync(Func<A, A> f) => Task.Run(() => Update(f));
        public Task<A> ResetAsync(A value) => UpdateAsync(_ => value);
        public Atom<B> Focus<B>(Expression<Func<A, B>> expr) => Focus(Lens.Of(expr));
        public Atom<B> Focus<B>(Func<A, B> get, Func<A, B, A> set) => Focus(Lens.Of(get, set));
        public Atom<B> Focus<B>(Lens<A, B> lens) => new FocusedAtom<A, B>(this, lens);
    }

    internal sealed class BasicAtom<A> : Atom<A>
    {
        private A value;

        public BasicAtom(A initial) => value = initial;

        internal override Lock Lock { get; } = Lock.New();

        public override A Update(Func<A, A> f) => Lock.Do(() => value = f(value));
    }

    internal sealed class FocusedAtom<A, B> : Atom<B>
    {
        private readonly Atom<A> target;
        private readonly Lens<A, B> lens;

        public FocusedAtom(Atom<A> target, Lens<A, B> lens)
        {
            this.target = target;
            this.lens = lens;
        }

        internal override Lock Lock => target.Lock;

        public override B Update(Func<B, B> f) =>
            lens.Get(target.Update(x => lens.Set(x, f(lens.Get(x)))));
    }

    internal sealed class ZippedAtom<A, B, Z> : Atom<Z>
    {
        private readonly Atom<A> atomA;
        private readonly Atom<B> atomB;
        private readonly Func<A, B, Z> combine;
        private readonly Func<Z, (A, B)> split;

        public ZippedAtom(
            Atom<A> atomA,
            Atom<B> atomB,
            Func<A, B, Z> combine,
            Func<Z, (A, B)> split)
        {
            this.atomA = atomA;
            this.atomB = atomB;
            this.combine = combine;
            this.split = split;
            Lock = Lock.Of(atomA.Lock, atomB.Lock);
        }

        internal override Lock Lock { get; }

        public override Z Update(Func<Z, Z> f) =>
            Lock.Do(() =>
            {
                var z = f(combine(atomA.Value, atomB.Value));
                (atomA.Value, atomB.Value) = split(z);
                return z;
            });
    }

    internal sealed class ZippedAtom<A, B, C, Z> : Atom<Z>
    {
        private readonly Atom<A> atomA;
        private readonly Atom<B> atomB;
        private readonly Atom<C> atomC;
        private readonly Func<A, B, C, Z> combine;
        private readonly Func<Z, (A, B, C)> split;

        public ZippedAtom(
            Atom<A> atomA,
            Atom<B> atomB,
            Atom<C> atomC,
            Func<A, B, C, Z> combine,
            Func<Z, (A, B, C)> split)
        {
            this.atomA = atomA;
            this.atomB = atomB;
            this.atomC = atomC;
            this.combine = combine;
            this.split = split;
            Lock = Lock.Of(atomA.Lock, atomB.Lock, atomC.Lock);
        }

        internal override Lock Lock { get; }

        public override Z Update(Func<Z, Z> f) =>
            Lock.Do(() =>
            {
                var z = f(combine(atomA.Value, atomB.Value, atomC.Value));
                (atomA.Value, atomB.Value, atomC.Value) = split(z);
                return z;
            });
    }

    internal sealed class ZippedAtom<A, B, C, D, Z> : Atom<Z>
    {
        private readonly Atom<A> atomA;
        private readonly Atom<B> atomB;
        private readonly Atom<C> atomC;
        private readonly Atom<D> atomD;
        private readonly Func<A, B, C, D, Z> combine;
        private readonly Func<Z, (A, B, C, D)> split;

        public ZippedAtom(
            Atom<A> atomA,
            Atom<B> atomB,
            Atom<C> atomC,
            Atom<D> atomD,
            Func<A, B, C, D, Z> combine,
            Func<Z, (A, B, C, D)> split)
        {
            this.atomA = atomA;
            this.atomB = atomB;
            this.atomC = atomC;
            this.atomD = atomD;
            this.combine = combine;
            this.split = split;
            Lock = Lock.Of(atomA.Lock, atomB.Lock, atomC.Lock, atomD.Lock);
        }

        internal override Lock Lock { get; }

        public override Z Update(Func<Z, Z> f) =>
            Lock.Do(() =>
            {
                var z = f(combine(atomA.Value, atomB.Value, atomC.Value, atomD.Value));
                (atomA.Value, atomB.Value, atomC.Value, atomD.Value) = split(z);
                return z;
            });
    }
}
