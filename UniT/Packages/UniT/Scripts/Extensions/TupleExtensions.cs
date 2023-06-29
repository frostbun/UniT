namespace UniT.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class TupleExtensions
    {
        public static IEnumerable<(TFirst, TSecond)> Where<TFirst, TSecond>(this IEnumerable<(TFirst, TSecond)> enumerable, Func<TFirst, TSecond, bool> predicate)
        {
            return enumerable.Where(tuple => predicate(tuple.Item1, tuple.Item2));
        }

        public static IEnumerable<TResult> Select<TFirst, TSecond, TResult>(this IEnumerable<(TFirst, TSecond)> enumerable, Func<TFirst, TSecond, TResult> selector)
        {
            return enumerable.Select(tuple => selector(tuple.Item1, tuple.Item2));
        }

        public static void ForEach<TFirst, TSecond>(this IEnumerable<(TFirst, TSecond)> enumerable, Action<TFirst, TSecond> action)
        {
            enumerable.ForEach(tuple => action(tuple.Item1, tuple.Item2));
        }

        public static bool All<TFirst, TSecond>(this IEnumerable<(TFirst, TSecond)> enumerable, Func<TFirst, TSecond, bool> predicate)
        {
            return enumerable.All(tuple => predicate(tuple.Item1, tuple.Item2));
        }

        public static bool Any<TFirst, TSecond>(this IEnumerable<(TFirst, TSecond)> enumerable, Func<TFirst, TSecond, bool> predicate)
        {
            return enumerable.Any(tuple => predicate(tuple.Item1, tuple.Item2));
        }

        public static Dictionary<TFirst, TSecond> ToDictionary<TFirst, TSecond>(this IEnumerable<(TFirst, TSecond)> enumerable)
        {
            return enumerable.ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
        }
    }
}