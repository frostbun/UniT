namespace UniT.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class CollectionExtensions
    {
        public static Stack<T> ToStack<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Aggregate(new Stack<T>(), (stack, item) =>
            {
                stack.Push(item);
                return stack;
            });
        }

        public static T PeekOrDefault<T>(this Stack<T> stack, Func<T> valueFactory = null)
        {
            return stack.Count > 0 ? stack.Peek() : (valueFactory ?? (() => default))();
        }

        public static T PopOrDefault<T>(this Stack<T> stack, Func<T> valueFactory = null)
        {
            return stack.Count > 0 ? stack.Pop() : (valueFactory ?? (() => default))();
        }

        public static Queue<T> ToQueue<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Aggregate(new Queue<T>(), (queue, item) =>
            {
                queue.Enqueue(item);
                return queue;
            });
        }

        public static T PeekOrDefault<T>(this Queue<T> queue, Func<T> valueFactory = null)
        {
            return queue.Count > 0 ? queue.Peek() : (valueFactory ?? (() => default))();
        }

        public static T DequeueOrDefault<T>(this Queue<T> queue, Func<T> valueFactory = null)
        {
            return queue.Count > 0 ? queue.Dequeue() : (valueFactory ?? (() => default))();
        }
    }
}