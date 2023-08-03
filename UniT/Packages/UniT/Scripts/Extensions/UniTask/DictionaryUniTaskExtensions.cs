namespace UniT.Extensions.UniTask
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;

    public static class DictionaryUniTaskExtensions
    {
        private static readonly HashSet<object> Locks = new();

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
            if (Locks.Contains(@lock)) return UniTask.WaitUntil(() => !Locks.Contains(@lock)).ContinueWith(() => false);
            Locks.Add(@lock);
            return valueFactory().ContinueWith(value =>
            {
                dictionary.Add(key, value);
                return true;
            }).Finally(() =>
            {
                Locks.Remove(@lock);
            });
        }
    }
}