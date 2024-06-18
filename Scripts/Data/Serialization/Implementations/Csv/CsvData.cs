#if UNIT_CSV
#nullable enable
namespace UniT.Data.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UniT.Extensions;
    using UnityEngine.Scripting;

    public abstract class CsvData : ICsvData
    {
        Type ICsvData.RowType => this.GetType();

        void ICsvData.Add(object key, object value) => value.CopyTo(this);

        IEnumerator ICsvData.GetEnumerator() { yield return this; }
    }

    public class CsvData<T> : ICsvData, IReadOnlyList<T>
    {
        [Preserve]
        public CsvData()
        {
        }

        Type ICsvData.RowType => typeof(T);

        void ICsvData.Add(object key, object value) => this.list.Add((T)value);

        IEnumerator ICsvData.GetEnumerator() => this.list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.list.GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => this.list.GetEnumerator();

        private readonly List<T> list = new List<T>();

        public int Count => this.list.Count;

        public T this[int index] => this.list[index];
    }

    public class CsvData<TKey, TValue> : ICsvData, IReadOnlyDictionary<TKey, TValue>
    {
        [Preserve]
        public CsvData()
        {
        }

        Type ICsvData.RowType => typeof(TValue);

        void ICsvData.Add(object key, object value) => this.dictionary.Add((TKey)key, (TValue)value);

        IEnumerator ICsvData.GetEnumerator() => this.dictionary.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.dictionary.GetEnumerator();

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => this.dictionary.GetEnumerator();

        private readonly Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        public int Count => this.dictionary.Count;

        public TValue this[TKey key] => this.dictionary[key];

        public IEnumerable<TKey> Keys => this.dictionary.Keys;

        public IEnumerable<TValue> Values => this.dictionary.Values;

        public bool ContainsKey(TKey key) => this.dictionary.ContainsKey(key);

        public bool TryGetValue(TKey key, out TValue value) => this.dictionary.TryGetValue(key, out value);
    }
}
#endif