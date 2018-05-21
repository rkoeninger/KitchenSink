using System;
using System.Collections.Generic;
using System.Linq;
using static KitchenSink.Operators;

namespace KitchenSink.Extensions
{
    public static class EnumerableMaybeExtensions
    {
        /// <summary>
        /// Attempts lookup at index, returning None if out of bounds.
        /// </summary>
        public static Maybe<A> GetMaybe<A>(this IReadOnlyList<A> list, int index) =>
            0 <= Cmp(index) < list.Count ? Some(list[index]) : None<A>();

        /// <summary>
        /// Attempts lookup at index, returning None if out of bounds.
        /// </summary>
        public static Maybe<A> GetMaybe<A>(this IList<A> list, int index) =>
            0 <= Cmp(index) < list.Count ? Some(list[index]) : None<A>();

        /// <summary>
        /// Attempts lookup by key, returning None if not found.
        /// </summary>
        public static Maybe<B> GetMaybe<A, B>(this IDictionary<A, B> dict, A key) =>
            dict.TryGetValue(key, out var value) ? Some(value) : None<B>();

        /// <summary>
        /// Attempts to get first element in sequence, returning None if empty.
        /// </summary>
        public static Maybe<A> FirstMaybe<A>(this IEnumerable<A> seq)
        {
            foreach (var item in seq)
            {
                return Some(item);
            }

            return None<A>();
        }

        /// <summary>
        /// Attempts to get last element in sequence, returning None if empty.
        /// </summary>
        public static Maybe<A> LastMaybe<A>(this IEnumerable<A> seq)
        {
            A result = default;
            var nonEmpty = false;

            foreach (var item in seq)
            {
                nonEmpty = true;
                result = item;
            }

            return Maybe.If(result, _ => nonEmpty);
        }

        /// <summary>
        /// Attempts to get first element in sequence that
        /// satisfies predicate, returning None if empty.
        /// </summary>
        public static Maybe<A> FirstMaybe<A>(this IEnumerable<A> seq, Func<A, bool> predicate)
        {
            foreach (var item in seq)
            {
                if (predicate(item))
                {
                    return Some(item);
                }
            }

            return None<A>();
        }

        /// <summary>
        /// Attempts to get first element in sequence with Some value,
        /// returning None if no elements do.
        /// </summary>
        public static Maybe<A> FirstSome<A>(this IEnumerable<Maybe<A>> seq)
        {
            foreach (var item in seq)
            {
                if (item.HasValue)
                {
                    return item;
                }
            }

            return None<A>();
        }

        /// <summary>
        /// Attempts to get first element in sequence with Some value,
        /// applying a selector function to each one first,
        /// returning None if no elements do.
        /// </summary>
        public static Maybe<B> FirstSome<A, B>(this IEnumerable<A> seq, Func<A, Maybe<B>> selector)
        {
            return FirstSome(seq.Select(selector));
        }

        /// <summary>
        /// Attempts to get first element in sequence with Some value
        /// which also satisfies predicate, returning None if no elements do.
        /// </summary>
        public static Maybe<A> FirstSome<A>(this IEnumerable<Maybe<A>> seq, Func<A, bool> predicate)
        {
            foreach (var item in seq)
            {
                if (item.HasValue && predicate(item.Value))
                {
                    return item;
                }
            }

            return None<A>();
        }

        /// <summary>
        /// Filters elements that have Some value and return those values.
        /// </summary>
        public static IEnumerable<A> WhereSome<A>(this IEnumerable<Maybe<A>> seq) =>
            seq.Where(x => x.HasValue).Select(x => x.Value);

        /// <summary>
        /// Applies selector to each element and returns results that have Some value.
        /// </summary>
        public static IEnumerable<B> SelectMany<A, B>(this IEnumerable<A> seq, Func<A, Maybe<B>> selector) =>
            seq.Select(selector).WhereSome();
    }
}
