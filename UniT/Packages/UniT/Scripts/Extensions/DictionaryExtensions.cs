namespace UniT.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public static class DictionaryExtensions
    {
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueFactory = null)
        {
            return dictionary.TryGetValue(key, out var value) ? value : (valueFactory ?? (() => default))();
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueFactory = null)
        {
            dictionary.TryAdd(key, valueFactory ?? (() => default));
            return dictionary[key];
        }

        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueFactory)
        {
            if (dictionary.ContainsKey(key)) return false;
            dictionary.Add(key, valueFactory());
            return true;
        }

        public static IEnumerable<KeyValuePair<TKey, TValue>> Where<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Func<TKey, TValue, bool> predicate)
        {
            return dictionary.Where(kv => predicate(kv.Key, kv.Value));
        }

        public static IEnumerable<TResult> Select<TKey, TValue, TResult>(this IDictionary<TKey, TValue> dictionary, Func<TKey, TValue, TResult> selector)
        {
            return dictionary.Select(kv => selector(kv.Key, kv.Value));
        }

        public static TResult Aggregate<TKey, TValue, TResult>(this IDictionary<TKey, TValue> dictionary, TResult seed, Func<TResult, TKey, TValue, TResult> func)
        {
            return dictionary.Aggregate(seed, (current, kv) => func(current, kv.Key, kv.Value));
        }

        public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Action<TKey, TValue> action)
        {
            dictionary.ForEach(kv => action(kv.Key, kv.Value));
        }

        public static int RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Func<TKey, TValue, bool> predicate)
        {
            var count = 0;
            foreach (var (key, value) in dictionary.Clone())
            {
                if (!predicate(key, value)) continue;
                dictionary.Remove(key);
                count++;
            }
            return count;
        }

        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return new(dictionary);
        }

        public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return new(dictionary);
        }

        public static Dictionary<TKey, TValue> ToDictionaryOneToOne<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> valueSelector)
        {
            var dictionary = new Dictionary<TKey, TValue>();
            foreach (var item in source)
            {
                dictionary.Add(keySelector(item), valueSelector(item));
            }
            return dictionary;
        }

        public static Dictionary<TKey, TValue> ToDictionaryOneToMany<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, IEnumerable<TValue>> valuesSelector)
        {
            var dictionary = new Dictionary<TKey, TValue>();
            foreach (var item in source)
            {
                var key = keySelector(item);
                foreach (var value in valuesSelector(item))
                {
                    dictionary.Add(key, value);
                }
            }
            return dictionary;
        }

        public static Dictionary<TKey, TValue> ToDictionaryManyToOne<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TKey>> keysSelector, Func<TSource, TValue> valueSelector)
        {
            var dictionary = new Dictionary<TKey, TValue>();
            foreach (var item in source)
            {
                var value = valueSelector(item);
                foreach (var key in keysSelector(item))
                {
                    dictionary.Add(key, value);
                }
            }
            return dictionary;
        }

        public static Dictionary<TKey, TValue> ToDictionaryManyToMany<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TKey>> keysSelector, Func<TSource, IEnumerable<TValue>> valuesSelector)
        {
            var dictionary = new Dictionary<TKey, TValue>();
            foreach (var item in source)
            {
                foreach (var key in keysSelector(item))
                {
                    foreach (var value in valuesSelector(item))
                    {
                        dictionary.Add(key, value);
                    }
                }
            }
            return dictionary;
        }
    }
}