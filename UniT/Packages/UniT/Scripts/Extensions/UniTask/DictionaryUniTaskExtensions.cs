namespace UniT.Extensions.UniTask
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;

    public static class DictionaryUniTaskExtensions
    {
        private static readonly HashSet<object> locks = new();

        public static UniTask<TValue> GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<UniTask<TValue>> valueFactory)
        {
            return dictionary.TryGetValue(key, out var value) ? UniTask.FromResult(value) : valueFactory();
        }

        public static UniTask<TValue> GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<UniTask<TValue>> valueFactory)
        {
            return dictionary.TryAdd(key, valueFactory).ContinueWith(_ => dictionary[key]);
        }

        public static UniTask<bool> TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<UniTask<TValue>> valueFactory)
        {
            if (dictionary.ContainsKey(key)) return UniTask.FromResult(false);
            var @lock = (dictionary, key);
            if (locks.Contains(@lock)) return UniTask.WaitUntil(() => !locks.Contains(@lock)).ContinueWith(() => false);
            locks.Add(@lock);
            return valueFactory().ContinueWith(value =>
            {
                dictionary.Add(key, value);
                return true;
            }).Finally(() =>
            {
                locks.Remove(@lock);
            });
        }
    }
}