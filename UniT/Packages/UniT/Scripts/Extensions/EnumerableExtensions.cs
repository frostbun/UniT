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

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.OrderBy(_ => Guid.NewGuid());
        }

        public static IEnumerable<T> Cycle<T>(this IEnumerable<T> enumerable, int count = -1)
        {
            if (count == 0) yield break;
            var cache = new List<T>();
            foreach (var item in enumerable)
            {
                yield return item;
                cache.Add(item);
            }

            while (--count != 0)
            {
                foreach (var item in cache)
                {
                    yield return item;
                }
            }
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

        public static Stack<T> ToStack<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Aggregate(new Stack<T>(), (stack, item) =>
            {
                stack.Push(item);
                return stack;
            });
        }

        public static T PopOrDefault<T>(this Stack<T> stack, Func<T> defaultValueFactory = null)
        {
            return stack.Count > 0 ? stack.Pop() : (defaultValueFactory ?? (() => default))();
        }

        public static Queue<T> ToQueue<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Aggregate(new Queue<T>(), (queue, item) =>
            {
                queue.Enqueue(item);
                return queue;
            });
        }

        public static T DequeueOrDefault<T>(this Queue<T> queue, Func<T> defaultValueFactory = null)
        {
            return queue.Count > 0 ? queue.Dequeue() : (defaultValueFactory ?? (() => default))();
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