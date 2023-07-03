namespace UniT.Extensions.UniTask
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;

    public static class DictionaryUniTaskExtensions
    {
        private static readonly HashSet<(IDictionary, object)> tasks = new();

        public static UniTask<TValue> GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<UniTask<TValue>> valueFactory)
        {
            return dictionary.TryGetValue(key, out var value) ? UniTask.FromResult(value) : valueFactory();
        }

        public static UniTask<TValue> GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<UniTask<TValue>> valueFactory)
        {
            return dictionary.TryAdd(key, valueFactory).ContinueWith(_ => dictionary[key]);
        }

        public static async UniTask<bool> TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<UniTask<TValue>> valueFactory)
        {
            if (dictionary.ContainsKey(key)) return false;
            var taskKey = ((IDictionary)dictionary, key);
            if (tasks.Contains(taskKey))
            {
                await UniTask.WaitUntil(() => !tasks.Contains(taskKey));
                return false;
            }
            tasks.Add(taskKey);
            try
            {
                dictionary.Add(key, await valueFactory());
                return true;
            }
            finally
            {
                tasks.Remove(taskKey);
            }
        }
    }
}