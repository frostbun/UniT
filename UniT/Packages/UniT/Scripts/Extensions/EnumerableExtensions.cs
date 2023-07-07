namespace UniT.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableExtension
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);
            }
        }

        public static (List<T>, List<T>) Split<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            return enumerable.Aggregate((new List<T>(), new List<T>()), (lists, item) =>
            {
                if (predicate(item)) lists.Item1.Add(item);
                else lists.Item2.Add(item);
                return lists;
            });
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.OrderBy(_ => Guid.NewGuid());
        }

        public static IEnumerable<T> Sample<T>(this IEnumerable<T> enumerable, int count)
        {
            return enumerable.Shuffle().Take(count);
        }

        public static T Choice<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Shuffle().First();
        }

        public static IEnumerable<T> Cycle<T>(this IEnumerable<T> enumerable)
        {
            var cache = new List<T>();
            foreach (var item in enumerable)
            {
                yield return item;
                cache.Add(item);
            }

            while (cache.Count > 0)
            {
                foreach (var item in cache)
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<(int, T)> Enumerate<T>(this IEnumerable<T> enumerable, int start = 0)
        {
            return enumerable.Select(item => (start++, item));
        }

        public static IEnumerable<T> Slice<T>(this IEnumerable<T> enumerable, int start, int stop, int step = 1)
        {
            var index = 0;
            foreach (var item in enumerable)
            {
                if (index >= stop) yield break;
                if (index >= start && (index - start) % step == 0) yield return item;
                ++index;
            }
        }

        public static T[,] To2DArray<T>(this T[][] source)
        {
            try
            {
                var dimension1 = source.Length;
                var dimension2 = source.GroupBy(row => row.Length).Single().Key;
                var result     = new T[dimension1, dimension2];
                for (var i = 0; i < dimension1; ++i)
                {
                    for (var j = 0; j < dimension2; ++j)
                    {
                        result[i, j] = source[i][j];
                    }
                }

                return result;
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("The given jagged array is not rectangular");
            }
        }
    }
}