namespace UniT.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class CollectionExtensions
    {
        public static bool RemoveFirst<T>(this ICollection<T> collection, Predicate<T> predicate)
        {
            foreach (var item in collection)
            {
                if (!predicate(item)) continue;
                collection.Remove(item);
                return true;
            }

            return false;
        }

        public static bool RemoveLast<T>(this ICollection<T> collection, Predicate<T> predicate)
        {
            foreach (var item in collection.Reverse())
            {
                if (!predicate(item)) continue;
                collection.Remove(item);
                return true;
            }

            return false;
        }
    }
}