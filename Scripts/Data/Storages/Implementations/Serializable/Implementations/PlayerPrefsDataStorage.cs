namespace UniT.Data
{
    using System;
    using System.Linq;
    using UniT.Extensions;
    using UnityEngine;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public sealed class PlayerPrefsDataStorage : SerializableDataStorage, IReadableSerializableDataStorage, IWritableSerializableDataStorage
    {
        [Preserve]
        public PlayerPrefsDataStorage()
        {
        }

        protected override bool CanStore(Type type) => base.CanStore(type)
            && typeof(IReadableData).IsAssignableFrom(type)
            && typeof(IWritableData).IsAssignableFrom(type);

        string[] IReadableSerializableDataStorage.Load(string[] keys) => Load(keys);

        void IWritableSerializableDataStorage.Save(string[] keys, string[] values) => Save(keys, values);

        void IWritableDataStorage.Flush() => Flush();

        #if UNIT_UNITASK
        UniTask<string[]> IReadableSerializableDataStorage.LoadAsync(string[] keys, IProgress<float> progress, CancellationToken cancellationToken)
        {
            var rawDatas = Load(keys);
            progress?.Report(1);
            return UniTask.FromResult(rawDatas);
        }

        UniTask IWritableSerializableDataStorage.SaveAsync(string[] keys, string[] values, IProgress<float> progress, CancellationToken cancellationToken)
        {
            Save(keys, values);
            progress?.Report(1);
            return UniTask.CompletedTask;
        }

        UniTask IWritableDataStorage.FlushAsync(IProgress<float> progress, CancellationToken cancellationToken)
        {
            Flush();
            progress?.Report(1);
            return UniTask.CompletedTask;
        }
        #else
        IEnumerator IReadableSerializableDataStorage.LoadAsync(string[] keys, Action<string[]> callback, IProgress<float> progress)
        {
            var rawDatas = Load(keys);
            progress?.Report(1);
            callback(rawDatas);
            yield break;
        }

        IEnumerator IWritableSerializableDataStorage.SaveAsync(string[] keys, string[] values, Action callback, IProgress<float> progress)
        {
            Save(keys, values);
            progress?.Report(1);
            callback?.Invoke();
            yield break;
        }

        IEnumerator IWritableDataStorage.FlushAsync(Action callback, IProgress<float> progress)
        {
            Flush();
            progress?.Report(1);
            callback?.Invoke();
            yield break;
        }
        #endif

        private static string[] Load(string[] keys)
        {
            return keys.Select(PlayerPrefs.GetString).ToArray();
        }

        private static void Save(string[] keys, string[] values)
        {
            IterTools.StrictZip(keys, values).ForEach(PlayerPrefs.SetString);
        }

        private static void Flush()
        {
            PlayerPrefs.Save();
        }
    }
}