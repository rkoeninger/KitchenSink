using System;
using System.Collections.Generic;
using System.Linq;
using KitchenSink.Purity;

namespace KitchenSink
{
    /// <summary>
    /// Transforms a function into one that can be mapped over a particular type.
    /// </summary>
    public static class Functor
    {
        /// <summary>
        /// Creates a new Functor from the given function.
        /// </summary>
        public static Functor<FA, FB, A, B> Of<FA, FB, A, B>(Func<Func<A, B>, Func<FA, FB>> selector) =>
            new Functor<FA, FB, A, B>(selector);

        /// <summary>
        /// Maps a function over an <see cref="System.Collections.Generic.IEnumerable{A}"/>.
        /// </summary>
        public static Functor<IEnumerable<A>, IEnumerable<B>, A, B> Enumerable<A, B>() =>
            Of<IEnumerable<A>, IEnumerable<B>, A, B>(f => xs => xs.Select(f));

        /// <summary>
        /// Maps a function over a <see cref="System.Collections.Generic.List{A}"/>.
        /// </summary>
        public static Functor<List<A>, List<B>, A, B> List<A, B>() =>
            Of<List<A>, List<B>, A, B>(f => xs => xs.Select(f).ToList());

        /// <summary>
        /// Maps a function over a <see cref="KitchenSink.Maybe{A}"/>.
        /// </summary>
        public static Functor<Maybe<A>, Maybe<B>, A, B> Maybe<A, B>() =>
            Of<Maybe<A>, Maybe<B>, A, B>(f => m => m.Select(f));

        /// <summary>
        /// Maps a function over the left value of an <see cref="KitchenSink.Either{A, B}"/>.
        /// </summary>
        public static Functor<Either<A, C>, Either<B, C>, A, B> EitherLeft<A, B, C>() =>
            Of<Either<A, C>, Either<B, C>, A, B>(f => m => m.SelectLeft(f));

        /// <summary>
        /// Maps a function over the right value of an <see cref="KitchenSink.Either{A, B}"/>.
        /// </summary>
        public static Functor<Either<C, A>, Either<C, B>, A, B> EitherRight<A, B, C>() =>
            Of<Either<C, A>, Either<C, B>, A, B>(f => m => m.SelectRight(f));

        /// <summary>
        /// Maps a function over an <see cref="Purity.IO{A}"/> action.
        /// </summary>
        public static Functor<IO<A>, IO<B>, A, B> IO<A, B>() =>
            Of<IO<A>, IO<B>, A, B>(f => m => m.Select(f));
    }

    /// <summary>
    /// Transforms a function into one that can be mapped over a particular type.
    /// </summary>
    public class Functor<FA, FB, A, B>
    {
        private readonly Func<Func<A, B>, Func<FA, FB>> selector;

        public Functor(Func<Func<A, B>, Func<FA, FB>> selector) => this.selector = selector;

        /// <summary>
        /// Lifts given function into space described by this Functor.
        /// </summary>
        public Func<FA, FB> Select(Func<A, B> f) => selector(f);
    }
}
