namespace UniT.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Cysharp.Threading.Tasks;

    public static class DictionaryExtensions
    {
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueFactory = null)
        {
            return dictionary.TryGetValue(key, out var value) ? value : (valueFactory ?? (() => default))();
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueFactory)
        {
            dictionary.TryAdd(key, valueFactory);
            return dictionary[key];
        }

        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueFactory)
        {
            if (dictionary.ContainsKey(key)) return false;
            dictionary[key] = valueFactory();
            return true;
        }

        public static UniTask<TValue> GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<UniTask<TValue>> valueFactory)
        {
            return dictionary.TryGetValue(key, out var value) ? UniTask.FromResult(value) : valueFactory();
        }

        public static UniTask<TValue> GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<UniTask<TValue>> valueFactory) where TValue : class
        {
            return dictionary.TryAdd(key, valueFactory).ContinueWith(_ => dictionary[key]);
        }

        public static UniTask<bool> TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<UniTask<TValue>> valueFactory) where TValue : class
        {
            return dictionary.TryAdd(key, (TValue)null)
                ? valueFactory().ContinueWith(value =>
                {
                    dictionary[key] = value;
                    return true;
                })
                : UniTask.WaitUntil(() => dictionary[key] != null).ContinueWith(() => false);
        }

        public static IEnumerable<KeyValuePair<TKey, TValue>> Where<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Func<TKey, TValue, bool> predicate)
        {
            foreach (var kv in dictionary)
            {
                if (!predicate(kv.Key, kv.Value)) continue;
                yield return kv;
            }
        }

        public static IEnumerable<TResult> Select<TKey, TValue, TResult>(this IDictionary<TKey, TValue> dictionary, Func<TKey, TValue, TResult> selector)
        {
            foreach (var (key, value) in dictionary)
            {
                yield return selector(key, value);
            }
        }

        public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Action<TKey, TValue> action)
        {
            foreach (var (key, value) in dictionary)
            {
                action(key, value);
            }
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
            return dictionary.ToDictionaryOneToOne(kv => kv.Key, kv => kv.Value);
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