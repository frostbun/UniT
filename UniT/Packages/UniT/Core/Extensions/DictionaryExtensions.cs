namespace UniT.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public static class DictionaryExtensions
    {
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValueFactory = null)
        {
            return dictionary.ContainsKey(key) ? dictionary[key] : (defaultValueFactory ?? (() => default))();
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueFactory = null)
        {
            return dictionary[key] = dictionary.GetOrDefault(key, valueFactory);
        }

        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueFactory)
        {
            if (dictionary.ContainsKey(key)) return false;
            dictionary[key] = valueFactory();
            return true;
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
                dictionary[keySelector(item)] = valueSelector(item);
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
                    dictionary[key] = value;
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
                    dictionary[key] = value;
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
                        dictionary[key] = value;
                    }
                }
            }

            return dictionary;
        }
    }
}