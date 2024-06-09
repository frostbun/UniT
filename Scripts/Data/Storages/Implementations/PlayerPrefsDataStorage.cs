#nullable enable
namespace UniT.Data.Storage
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

    public sealed class PlayerPrefsDataStorage : IReadableStringStorage, IWritableStringStorage
    {
        [Preserve]
        public PlayerPrefsDataStorage()
        {
        }

        bool IDataStorage.CanStore(Type type) => typeof(IReadableData).IsAssignableFrom(type) && typeof(IWritableData).IsAssignableFrom(type);

        string[] IReadableStringStorage.Read(string[] keys) => Read(keys);

        void IWritableStringStorage.Write(string[] keys, string[] values) => Write(keys, values);

        void IWritableDataStorage.Flush() => Flush();

        #if UNIT_UNITASK
        UniTask<string[]> IReadableStringStorage.ReadAsync(string[] keys, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            var rawDatas = Read(keys);
            progress?.Report(1);
            return UniTask.FromResult(rawDatas);
        }

        UniTask IWritableStringStorage.WriteAsync(string[] keys, string[] values, IProgress<float>? progress, CancellationToken cancellationToken)
        {
            Write(keys, values);
            progress?.Report(1);
            return UniTask.CompletedTask;
        }

        UniTask IWritableDataStorage.FlushAsync(IProgress<float>? progress, CancellationToken cancellationToken)
        {
            Flush();
            progress?.Report(1);
            return UniTask.CompletedTask;
        }
        #else
        IEnumerator IReadableStringStorage.ReadAsync(string[] keys, Action<string[]> callback, IProgress<float>? progress)
        {
            var rawDatas = Read(keys);
            progress?.Report(1);
            callback(rawDatas);
            yield break;
        }

        IEnumerator IWritableStringStorage.WriteAsync(string[] keys, string[] values, Action? callback, IProgress<float>? progress)
        {
            Write(keys, values);
            progress?.Report(1);
            callback?.Invoke();
            yield break;
        }

        IEnumerator IWritableDataStorage.FlushAsync(Action? callback, IProgress<float>? progress)
        {
            Flush();
            progress?.Report(1);
            callback?.Invoke();
            yield break;
        }
        #endif

        private static string[] Read(string[] keys)
        {
            return keys.Select(PlayerPrefs.GetString).ToArray();
        }

        private static void Write(string[] keys, string[] values)
        {
            IterTools.Zip(keys, values).ForEach(PlayerPrefs.SetString);
        }

        private static void Flush()
        {
            PlayerPrefs.Save();
        }
    }
}