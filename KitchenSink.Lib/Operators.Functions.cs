using System;
using System.Collections.Concurrent;
using KitchenSink.Concurrent;

namespace KitchenSink
{
    public static partial class Operators
    {
        /// <summary>
        /// Forward function composition.
        /// </summary>
        public static Func<A, C> Compose<A, B, C>(
            Func<A, B> f,
            Func<B, C> g) =>
            x => g(f(x));

        /// <summary>
        /// Forward function composition.
        /// </summary>
        public static Func<A, D> Compose<A, B, C, D>(
            Func<A, B> f,
            Func<B, C> g,
            Func<C, D> h) =>
            x => h(g(f(x)));

        /// <summary>
        /// Forward function composition.
        /// </summary>
        public static Func<A, E> Compose<A, B, C, D, E>(
            Func<A, B> f,
            Func<B, C> g,
            Func<C, D> h,
            Func<D, E> i) =>
            x => i(h(g(f(x))));

        /// <summary>
        /// Forward function composition.
        /// </summary>
        public static Func<A, F> Compose<A, B, C, D, E, F>(
            Func<A, B> f,
            Func<B, C> g,
            Func<C, D> h,
            Func<D, E> i,
            Func<E, F> j) =>
            x => j(i(h(g(f(x)))));

        /// <summary>
        /// Function currying.
        /// </summary>
        public static Func<A, Func<B, C>> Curry<A, B, C>(
            Func<A, B, C> f) =>
            x => y => f(x, y);

        /// <summary>
        /// Function currying.
        /// </summary>
        public static Func<A, Func<B, Func<C, D>>> Curry<A, B, C, D>(
            Func<A, B, C, D> f) =>
            x => y => z => f(x, y, z);

        /// <summary>
        /// Function currying.
        /// </summary>
        public static Func<A, Func<B, Func<C, Func<D, E>>>> Curry<A, B, C, D, E>(
            Func<A, B, C, D, E> f) =>
            x => y => z => w => f(x, y, z, w);

        /// <summary>
        /// Function un-currying.
        /// </summary>
        public static Func<A, B, C> Uncurry<A, B, C>(
            Func<A, Func<B, C>> f) =>
            (x, y) => f(x)(y);

        /// <summary>
        /// Function un-currying.
        /// </summary>
        public static Func<A, B, C, D> Uncurry<A, B, C, D>(
            Func<A, Func<B, Func<C, D>>> f) =>
            (x, y, z) => f(x)(y)(z);

        /// <summary>
        /// Function un-currying.
        /// </summary>
        public static Func<A, B, C, D, E> Uncurry<A, B, C, D, E>(
            Func<A, Func<B, Func<C, Func<D, E>>>> f) =>
            (x, y, z, w) => f(x)(y)(z)(w);

        /// <summary>
        /// Join parameters into tuple.
        /// </summary>
        public static Func<(A, B), Z> Tuplize<A, B, Z>(Func<A, B, Z> f) =>
            t => f(t.Item1, t.Item2);

        /// <summary>
        /// Join parameters into tuple.
        /// </summary>
        public static Func<(A, B, C), Z> Tuplize<A, B, C, Z>(Func<A, B, C, Z> f) =>
            t => f(t.Item1, t.Item2, t.Item3);

        /// <summary>
        /// Join parameters into tuple.
        /// </summary>
        public static Func<(A, B, C, D), Z> Tuplize<A, B, C, D, Z>(Func<A, B, C, D, Z> f) =>
            t => f(t.Item1, t.Item2, t.Item3, t.Item4);

        /// <summary>
        /// Join parameters into tuple.
        /// </summary>
        public static Action<(A, B)> Tuplize<A, B>(Action<A, B> f) =>
            t => f(t.Item1, t.Item2);

        /// <summary>
        /// Join parameters into tuple.
        /// </summary>
        public static Action<(A, B, C)> Tuplize<A, B, C>(Action<A, B, C> f) =>
            t => f(t.Item1, t.Item2, t.Item3);

        /// <summary>
        /// Join parameters into tuple.
        /// </summary>
        public static Action<(A, B, C, D)> Tuplize<A, B, C, D>(Action<A, B, C, D> f) =>
            t => f(t.Item1, t.Item2, t.Item3, t.Item4);

        /// <summary>
        /// Split parameters from tuple.
        /// </summary>
        public static Func<A, B, Z> Detuplize<A, B, Z>(Func<(A, B), Z> f) =>
            (a, b) => f((a, b));

        /// <summary>
        /// Split parameters from tuple.
        /// </summary>
        public static Func<A, B, C, Z> Detuplize<A, B, C, Z>(Func<(A, B, C), Z> f) =>
            (a, b, c) => f((a, b, c));

        /// <summary>
        /// Split parameters from tuple.
        /// </summary>
        public static Func<A, B, C, D, Z> Detuplize<A, B, C, D, Z>(Func<(A, B, C, D), Z> f) =>
            (a, b, c, d) => f((a, b, c, d));

        /// <summary>
        /// Split parameters from tuple.
        /// </summary>
        public static Action<A, B> Detuplize<A, B>(Action<(A, B)> f) =>
            (a, b) => f((a, b));

        /// <summary>
        /// Split parameters from tuple.
        /// </summary>
        public static Action<A, B, C> Detuplize<A, B, C>(Action<(A, B, C)> f) =>
            (a, b, c) => f((a, b, c));

        /// <summary>
        /// Split parameters from tuple.
        /// </summary>
        public static Action<A, B, C, D> Detuplize<A, B, C, D>(Action<(A, B, C, D)> f) =>
            (a, b, c, d) => f((a, b, c, d));

        /// <summary>
        /// Wrap function with one that takes placeholder Unit.
        /// </summary>
        public static Func<Unit, Z> TakeUnit<Z>(Func<Z> f) => _ => f();

        /// <summary>
        /// Wrap function with one that takes placeholder Unit.
        /// </summary>
        public static Action<Unit> TakeUnit(Action f) => _ => f();

        /// <summary>
        /// Wrap function with one that gives placeholder Unit.
        /// </summary>
        public static Func<Z> GiveUnit<Z>(Func<Unit, Z> f) => () => f(Unit.It);

        /// <summary>
        /// Wrap function with one that gives placeholder Unit.
        /// </summary>
        public static Action GiveUnit(Action<Unit> f) => () => f(Unit.It);

        /// <summary>
        /// Partially apply function.
        /// </summary>
        public static Func<B, Z> Apply<A, B, Z>(Func<A, B, Z> f, A a) =>
            b => f(a, b);

        /// <summary>
        /// Partially apply function.
        /// </summary>
        public static Func<B, C, Z> Apply<A, B, C, Z>(Func<A, B, C, Z> f, A a) =>
            (b, c) => f(a, b, c);

        /// <summary>
        /// Partially apply function.
        /// </summary>
        public static Func<C, Z> Apply<A, B, C, Z>(Func<A, B, C, Z> f, A a, B b) =>
            c => f(a, b, c);

        /// <summary>
        /// Partially apply function.
        /// </summary>
        public static Func<B, C, D, Z> Apply<A, B, C, D, Z>(Func<A, B, C, D, Z> f, A a) =>
            (b, c, d) => f(a, b, c, d);

        /// <summary>
        /// Partially apply function.
        /// </summary>
        public static Func<C, D, Z> Apply<A, B, C, D, Z>(Func<A, B, C, D, Z> f, A a, B b) =>
            (c, d) => f(a, b, c, d);

        /// <summary>
        /// Partially apply function.
        /// </summary>
        public static Func<D, Z> Apply<A, B, C, D, Z>(Func<A, B, C, D, Z> f, A a, B b, C c) =>
            d => f(a, b, c, d);

        /// <summary>
        /// Flip function arguments.
        /// </summary>
        public static Func<B, A, Z> Flip<A, B, Z>(Func<A, B, Z> f) =>
            (b, a) => f(a, b);

        /// <summary>
        /// Rotate function arguments forward.
        /// </summary>
        public static Func<C, A, B, Z> Rotate<A, B, C, Z>(Func<A, B, C, Z> f) =>
            (c, a, b) => f(a, b, c);

        /// <summary>
        /// Rotate function arguments backward.
        /// </summary>
        public static Func<B, C, A, Z> RotateBack<A, B, C, Z>(Func<A, B, C, Z> f) =>
            (b, c, a) => f(a, b, c);

        /// <summary>
        /// Wrap function with memoizing cache.
        /// </summary>
        public static Func<A, Z> Memo<A, Z>(Func<A, Z> f)
        {
            var cache = new ConcurrentDictionary<A, Z>();
            return a => cache.GetOrAdd(a, f);
        }

        /// <summary>
        /// Wrap function with memoizing cache.
        /// </summary>
        public static Func<A, B, Z> Memo<A, B, Z>(Func<A, B, Z> f) =>
            Detuplize(Memo(Tuplize(f)));

        /// <summary>
        /// Wrap function with memoizing cache.
        /// </summary>
        public static Func<A, B, C, Z> Memo<A, B, C, Z>(Func<A, B, C, Z> f) =>
            Detuplize(Memo(Tuplize(f)));

        /// <summary>
        /// Wrap function with memoizing cache.
        /// </summary>
        public static Func<A, B, C, D, Z> Memo<A, B, C, D, Z>(Func<A, B, C, D, Z> f) =>
            Detuplize(Memo(Tuplize(f)));

        /// <summary>
        /// Wrap function with memoizing cache with an expiration timeout.
        /// </summary>
        public static Func<Z> Memo<Z>(TimeSpan timeout, Func<Z> f) =>
            GiveUnit(Memo(timeout, TakeUnit(f)));

        /// <summary>
        /// Wrap function with memoizing cache with an expiration timeout.
        /// </summary>
        public static Func<A, Z> Memo<A, Z>(TimeSpan timeout, Func<A, Z> f)
        {
            var cache = new ConcurrentDictionary<A, (DateTime, Z)>();
            return a => cache.AddOrUpdate(
                a,
                _ => (DateTime.UtcNow, f(a)),
                (_, current) => DateTime.UtcNow - current.Item1 > timeout
                    ? (DateTime.UtcNow, f(a))
                    : current).Item2;
        }

        /// <summary>
        /// Wrap function with memoizing cache with an expiration timeout.
        /// </summary>
        public static Func<A, B, Z> Memo<A, B, Z>(TimeSpan timeout, Func<A, B, Z> f) =>
            Detuplize(Memo(timeout, Tuplize(f)));

        /// <summary>
        /// Wrap function with memoizing cache with an expiration timeout.
        /// </summary>
        public static Func<A, B, C, Z> Memo<A, B, C, Z>(TimeSpan timeout, Func<A, B, C, Z> f) =>
            Detuplize(Memo(timeout, Tuplize(f)));

        /// <summary>
        /// Wrap function with memoizing cache with an expiration timeout.
        /// </summary>
        public static Func<A, B, C, D, Z> Memo<A, B, C, D, Z>(TimeSpan timeout, Func<A, B, C, D, Z> f) =>
            Detuplize(Memo(timeout, Tuplize(f)));

        /// <summary>
        /// Wraps action so it won't be called more frequently than given timeout.
        /// </summary>
        public static Action Debounce(TimeSpan timeout, Action f) =>
            GiveUnit(Debounce(timeout, TakeUnit(f)));

        /// <summary>
        /// Wraps action so it won't be called more frequently than given timeout.
        /// </summary>
        public static Action<A> Debounce<A>(TimeSpan timeout, Action<A> f)
        {
            var atom = Atom.Of(DateTime.MinValue);
            return a =>
            {
                atom.Update(prev =>
                {
                    if (DateTime.UtcNow - prev > timeout)
                    {
                        f(a);
                        return DateTime.UtcNow;
                    }

                    return prev;
                });
            };
        }

        /// <summary>
        /// Wraps action so it won't be called more frequently than given timeout.
        /// </summary>
        public static Action<A, B> Debounce<A, B>(TimeSpan timeout, Action<A, B> f) =>
            Detuplize(Debounce(timeout, Tuplize(f)));

        /// <summary>
        /// Wraps action so it won't be called more frequently than given timeout.
        /// </summary>
        public static Action<A, B, C> Debounce<A, B, C>(TimeSpan timeout, Action<A, B, C> f) =>
            Detuplize(Debounce(timeout, Tuplize(f)));

        /// <summary>
        /// Wraps action so it won't be called more frequently than given timeout.
        /// </summary>
        public static Action<A, B, C, D> Debounce<A, B, C, D>(TimeSpan timeout, Action<A, B, C, D> f) =>
            Detuplize(Debounce(timeout, Tuplize(f)));

        /// <summary>
        /// Returns an IDisposable that, when <c>Dispose</c>d, calls the given function.
        /// </summary>
        public static IDisposable Disposable(Action f) => new DisposableAction(f);

        internal class DisposableAction : IDisposable
        {
            private readonly Action f;

            internal DisposableAction(Action f) => this.f = f;

            public void Dispose() => f();
        }
    }
}
