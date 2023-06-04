namespace UniT.Extensions
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
    }
}