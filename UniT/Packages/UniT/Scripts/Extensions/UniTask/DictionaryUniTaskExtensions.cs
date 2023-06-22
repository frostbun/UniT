namespace UniT.Extensions.UniTask
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;

    public static class DictionaryUniTaskExtensions
    {
        private static readonly Dictionary<(IDictionary, object), UniTask> tasks = new();

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
            var taskKey = ((IDictionary)dictionary, key);
            if (tasks.TryGetValue(taskKey, out var task)) return task.ContinueWith(() => false);
            return (tasks[taskKey] = valueFactory().ContinueWith(value =>
            {
                dictionary[key] = value;
                tasks.Remove(taskKey);
            })).ContinueWith(() => true);
        }
    }
}