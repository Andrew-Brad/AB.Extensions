using System;
using System.Collections.Generic;
using System.Linq;

using AB.Extensions.Enums;

#if NETSTANDARD2_0
using System.Threading; // ThreadSafeRandom only; modern TFMs use Random.Shared
#endif

namespace AB.Extensions
{
    /// <summary>
    /// Credit goes to Jon Skeet and Atif Aziz at https://github.com/morelinq/MoreLINQ for these.  BRILLIANT.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Sorts the elements of a sequence in a particular direction (ascending, descending) according to a key
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence</typeparam>
        /// <typeparam name="TKey">The type of the key used to order elements</typeparam>
        /// <param name="source">The sequence to order</param>
        /// <param name="keySelector">A key selector function</param>
        /// <param name="direction">A direction in which to order the elements (ascending, descending)</param>
        /// <returns>An ordered copy of the source sequence</returns>

        public static IOrderedEnumerable<T> OrderBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector, OrderByDirection direction)
        {
            return OrderBy(source, keySelector, null, direction);
        }

        /// <summary>
        /// Sorts the elements of a sequence in a particular direction (ascending, descending) according to a key
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence</typeparam>
        /// <typeparam name="TKey">The type of the key used to order elements</typeparam>
        /// <param name="source">The sequence to order</param>
        /// <param name="keySelector">A key selector function</param>
        /// <param name="direction">A direction in which to order the elements (ascending, descending)</param>
        /// <param name="comparer">A comparer used to define the semantics of element comparison</param>
        /// <returns>An ordered copy of the source sequence</returns>

        public static IOrderedEnumerable<T> OrderBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector, IComparer<TKey>? comparer, OrderByDirection direction)
        {
            return direction == OrderByDirection.Ascending
                       ? source.OrderBy(keySelector, comparer)
                       : source.OrderByDescending(keySelector, comparer);
        }

        /// <summary>
        /// Performs a subsequent ordering of elements in a sequence in a particular direction (ascending, descending) according to a key
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence</typeparam>
        /// <typeparam name="TKey">The type of the key used to order elements</typeparam>
        /// <param name="source">The sequence to order</param>
        /// <param name="keySelector">A key selector function</param>
        /// <param name="direction">A direction in which to order the elements (ascending, descending)</param>
        /// <returns>An ordered copy of the source sequence</returns>

        public static IOrderedEnumerable<T> ThenBy<T, TKey>(this IOrderedEnumerable<T> source, Func<T, TKey> keySelector, OrderByDirection direction)
        {
            return ThenBy(source, keySelector, null, direction);
        }

        /// <summary>
        /// Performs a subsequent ordering of elements in a sequence in a particular direction (ascending, descending) according to a key
        /// </summary>
        /// <typeparam name="T">The type of the elements in the source sequence</typeparam>
        /// <typeparam name="TKey">The type of the key used to order elements</typeparam>
        /// <param name="source">The sequence to order</param>
        /// <param name="keySelector">A key selector function</param>
        /// <param name="direction">A direction in which to order the elements (ascending, descending)</param>
        /// <param name="comparer">A comparer used to define the semantics of element comparison</param>
        /// <returns>An ordered copy of the source sequence</returns>

        public static IOrderedEnumerable<T> ThenBy<T, TKey>(this IOrderedEnumerable<T> source, Func<T, TKey> keySelector, IComparer<TKey>? comparer, OrderByDirection direction)
        {
            return direction == OrderByDirection.Ascending
                       ? source.ThenBy(keySelector, comparer)
                       : source.ThenByDescending(keySelector, comparer);
        }

        /// <summary>
        /// Skips items from the input sequence until the given predicate returns true
        /// when applied to the current source item; that item will be the last skipped.
        /// </summary>
        /// <remarks>
        /// <para>
        /// SkipUntil differs from Enumerable.SkipWhile in two respects. Firstly, the sense
        /// of the predicate is reversed: it is expected that the predicate will return false
        /// to start with, and then return true - for example, when trying to find a matching
        /// item in a sequence.
        /// </para>
        /// <para>
        /// Secondly, SkipUntil skips the element which causes the predicate to return true. For
        /// example, in a sequence <code>{ 1, 2, 3, 4, 5 }</code> and with a predicate of
        /// <code>x => x == 3</code>, the result would be <code>{ 4, 5 }</code>.
        /// </para>
        /// <para>
        /// SkipUntil is as lazy as possible: it will not iterate over the source sequence
        /// until it has to, it won't iterate further than it has to, and it won't evaluate
        /// the predicate until it has to. (This means that an item may be returned which would
        /// actually cause the predicate to throw an exception if it were evaluated, so long as
        /// it comes after the first item causing the predicate to return true.)
        /// </para>
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="predicate">Predicate used to determine when to stop yielding results from the source.</param>
        /// <returns>Items from the source sequence after the predicate first returns true when applied to the item.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is null</exception>

        public static IEnumerable<TSource> SkipUntil<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return SkipUntilImpl(source, predicate);
        }

        private static IEnumerable<TSource> SkipUntilImpl<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            using (var iterator = source.GetEnumerator())
            {
                while (iterator.MoveNext())
                {
                    if (predicate(iterator.Current))
                    {
                        break;
                    }
                }
                while (iterator.MoveNext())
                {
                    yield return iterator.Current;
                }
            }
        }
        /// <summary>
        /// Returns items from the input sequence until the given predicate returns true
        /// when applied to the current source item; that item will be the last returned.
        /// </summary>
        /// <remarks>
        /// <para>
        /// TakeUntil differs from Enumerable.TakeWhile in two respects. Firstly, the sense
        /// of the predicate is reversed: it is expected that the predicate will return false
        /// to start with, and then return true - for example, when trying to find a matching
        /// item in a sequence.
        /// </para>
        /// <para>
        /// Secondly, TakeUntil yields the element which causes the predicate to return true. For
        /// example, in a sequence <code>{ 1, 2, 3, 4, 5 }</code> and with a predicate of
        /// <code>x => x == 3</code>, the result would be <code>{ 1, 2, 3 }</code>.
        /// </para>
        /// <para>
        /// TakeUntil is as lazy as possible: it will not iterate over the source sequence
        /// until it has to, it won't iterate further than it has to, and it won't evaluate
        /// the predicate until it has to. (This means that an item may be returned which would
        /// actually cause the predicate to throw an exception if it were evaluated, so long as
        /// no more items of data are requested.)
        /// </para>
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="predicate">Predicate used to determine when to stop yielding results from the source.</param>
        /// <returns>Items from the source sequence, until the predicate returns true when applied to the item.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is null</exception>

        public static IEnumerable<TSource> TakeUntil<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return TakeUntilImpl(source, predicate);
        }

        private static IEnumerable<TSource> TakeUntilImpl<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            foreach (var item in source)
            {
                yield return item;
                if (predicate(item))
                {
                    yield break;
                }
            }
        }

        /// <summary>
        /// Reorders elements in the IList based on a pseudo-random, thread-safe, environment tick count seed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        // Attribution: https://stackoverflow.com/questions/273313/randomize-a-listt-in-c-sharp
        public static void Shuffle<T>(this IList<T> list)
        {
#if NETSTANDARD2_0
            Random rng = ThreadSafeRandom.ThisThreadsRandom;
#else
            Random rng = Random.Shared; // thread-safe since .NET 6
#endif
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

#if NETSTANDARD2_0
        /// <summary>
        /// Provides a per-thread <see cref="Random"/> instance, avoiding the cross-thread
        /// state corruption that results from sharing a single <see cref="Random"/>.
        /// </summary>
        // netstandard2.0 only: modern TFMs use the thread-safe Random.Shared.
        public static class ThreadSafeRandom
        {
            [ThreadStatic]
            private static Random? Local;

            /// <summary>
            /// A <see cref="Random"/> instance unique to the calling thread.
            /// </summary>
            public static Random ThisThreadsRandom
            {
                get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
            }
        }
#endif

        //public static void NaiveNonRandomShuffle<T>(this IList<T> list)
        //{
        //    list.OrderBy(a => Guid.NewGuid());
        //}               

        /// <summary>
        /// Invokes an action for each element of a sequence.
        /// </summary>
        /// <typeparam name="T">The element type of the sequence.</typeparam>
        /// <param name="source">The sequence to enumerate.</param>
        /// <param name="action">The action to invoke for each element.</param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
                action(element);
        }

        /// <summary>
        /// Returns true if all elements are monotonically increasing in this IEnumerable. Non-strict.
        /// Will only enumerate a single time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static bool IsMonotonicallyIncreasing<T>(this IEnumerable<T> elements) where T : IComparable<T>
        {
            using (var e = elements.GetEnumerator())
            {
                if (!e.MoveNext())
                    return true;
                T prev = e.Current;
                while (e.MoveNext())
                {
                    if (prev.CompareTo(e.Current) > 0)
                        return false;
                    prev = e.Current;
                }
                return true;
            }
        }

#if NETSTANDARD2_0
        /// <summary>
        /// Returns the distinct elements of a sequence according to a projected key.
        /// </summary>
        /// <typeparam name="TSource">The element type of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TKey">The key type projected by <paramref name="keySelector"/>.</typeparam>
        /// <param name="source">The sequence to remove duplicates from.</param>
        /// <param name="keySelector">Projects each element to the key compared for equality.</param>
        /// <returns>A sequence containing the first element for each distinct key.</returns>
        // netstandard2.0 only: net6+ ships Enumerable.DistinctBy.
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
#endif
    }
}
