﻿using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Maps a function over an <see cref="IEnumerable{A}"/>.
        /// </summary>
        public static Functor<IEnumerable<A>, IEnumerable<B>, A, B> Enumerable<A, B>() =>
            Of<IEnumerable<A>, IEnumerable<B>, A, B>(f => xs => xs.Select(f));

        /// <summary>
        /// Maps a function over a <see cref="List{A}"/>.
        /// </summary>
        public static Functor<List<A>, List<B>, A, B> List<A, B>() =>
            Of<List<A>, List<B>, A, B>(f => xs => xs.Select(f).ToList());

        /// <summary>
        /// Maps a function over a <see cref="Maybe{A}"/>.
        /// </summary>
        public static Functor<Maybe<A>, Maybe<B>, A, B> Maybe<A, B>() =>
            Of<Maybe<A>, Maybe<B>, A, B>(f => m => m.Select(f));

        /// <summary>
        /// Maps a function over the left value of an <see cref="Either{A, B}"/>.
        /// </summary>
        public static Functor<Either<A, C>, Either<B, C>, A, B> EitherLeft<A, B, C>() =>
            Of<Either<A, C>, Either<B, C>, A, B>(f => m => m.SelectLeft(f));

        /// <summary>
        /// Maps a function over the right value of an <see cref="Either{A, B}"/>.
        /// </summary>
        public static Functor<Either<C, A>, Either<C, B>, A, B> EitherRight<A, B, C>() =>
            Of<Either<C, A>, Either<C, B>, A, B>(f => m => m.SelectRight(f));

        /// <summary>
        /// Maps a function over an <see cref="Io{A}"/> action.
        /// </summary>
        public static Functor<Io<A>, Io<B>, A, B> Io<A, B>() =>
            Of<Io<A>, Io<B>, A, B>(f => m => m.Select(f));

        /// <summary>
        /// Composes covariant function with transformation of its resulting value.
        /// </summary>
        public static Functor<Func<Z, A>, Func<Z, B>, A, B> Func<Z, A, B>() =>
            Of<Func<Z, A>, Func<Z, B>, A, B>(f => g => x => f(g(x)));
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
