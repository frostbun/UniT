#if ODIN_INSPECTOR
#nullable enable
namespace UniT.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Extensions;
    using UnityEngine;

    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private KeyValuePair[] keyValuePairs = Array.Empty<KeyValuePair>();

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            this.keyValuePairs = this.Select((key, value) => new KeyValuePair(key, value)).ToArray();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            this.Clear();
            this.keyValuePairs.ForEach(pair => this.Add(pair.Key, pair.Value));
        }

        [Serializable]
        private sealed class KeyValuePair
        {
            [field: SerializeField] public TKey   Key   { get; private set; }
            [field: SerializeField] public TValue Value { get; private set; }

            public KeyValuePair(TKey key, TValue value)
            {
                this.Key   = key;
                this.Value = value;
            }
        }
    }
}
#endif