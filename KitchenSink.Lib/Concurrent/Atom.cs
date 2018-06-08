using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KitchenSink.Purity;

namespace KitchenSink.Concurrent
{
    public static class Atom
    {
        public static Atom<A> Of<A>(A initial) => new BasicAtom<A>(initial);
    }

    public abstract class Atom<A>
    {
        internal Atom()
        {
        }

        internal abstract Lock Lock { get; }

        public abstract A Value { get; set; }
        public abstract A Update(Func<A, A> f);

        public A Reset(A value) => Update(_ => value);
        public Task<A> UpdateAsync(Func<A, A> f) => Task.Run(() => Update(f));
        public Task<A> ResetAsync(A value) => Task.Run(() => Reset(value));
        public Atom<B> Focus<B>(Expression<Func<A, B>> expr) => Focus(Lens.Of(expr));
        public Atom<B> Focus<B>(Func<A, B> get, Func<A, B, A> set) => Focus(Lens.Of(get, set));
        public Atom<B> Focus<B>(Lens<A, B> lens) => new FocusedAtom<A, B>(this, lens);
    }

    public sealed class BasicAtom<A> : Atom<A>
    {
        private A value;

        public BasicAtom(A initial) => value = initial;

        internal override Lock Lock { get; } = new Lock();

        public override A Value
        {
            get => value;
            set => Lock.Do(() => this.value = value);
        }

        public override A Update(Func<A, A> f) => Lock.Do(() => value = f(value));
    }

    public sealed class FocusedAtom<A, B> : Atom<B>
    {
        private readonly Atom<A> target;
        private readonly Lens<A, B> lens;

        public FocusedAtom(Atom<A> target, Lens<A, B> lens)
        {
            this.target = target;
            this.lens = lens;
        }

        internal override Lock Lock => target.Lock;

        public override B Value
        {
            get => lens.Get(target.Value);
            set => target.Update(x => lens.Set(x, value));
        }

        public override B Update(Func<B, B> f) =>
            lens.Get(target.Update(x => lens.Set(x, f(lens.Get(x)))));
    }
}
