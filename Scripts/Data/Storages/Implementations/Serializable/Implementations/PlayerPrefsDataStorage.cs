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

        string[] IReadableSerializableDataStorage.ReadStrings(string[] keys) => ReadStrings(keys);

        byte[][] IReadableSerializableDataStorage.ReadBytes(string[] keys) => ReadBytes(keys);

        void IWritableSerializableDataStorage.WriteStrings(string[] keys, string[] values) => WriteStrings(keys, values);

        void IWritableSerializableDataStorage.WriteBytes(string[] keys, byte[][] values) => WriteBytes(keys, values);

        void IWritableDataStorage.Flush() => Flush();

        #if UNIT_UNITASK
        UniTask<string[]> IReadableSerializableDataStorage.ReadStringsAsync(string[] keys, IProgress<float> progress, CancellationToken cancellationToken)
        {
            var rawDatas = ReadStrings(keys);
            progress?.Report(1);
            return UniTask.FromResult(rawDatas);
        }

        UniTask<byte[][]> IReadableSerializableDataStorage.ReadBytesAsync(string[] keys, IProgress<float> progress, CancellationToken cancellationToken)
        {
            var rawDatas = ReadBytes(keys);
            progress?.Report(1);
            return UniTask.FromResult(rawDatas);
        }

        UniTask IWritableSerializableDataStorage.WriteStringsAsync(string[] keys, string[] values, IProgress<float> progress, CancellationToken cancellationToken)
        {
            WriteStrings(keys, values);
            progress?.Report(1);
            return UniTask.CompletedTask;
        }

        UniTask IWritableSerializableDataStorage.WriteBytesAsync(string[] keys, byte[][] values, IProgress<float> progress, CancellationToken cancellationToken)
        {
            WriteBytes(keys, values);
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
        IEnumerator IReadableSerializableDataStorage.ReadStringsAsync(string[] keys, Action<string[]> callback, IProgress<float> progress)
        {
            var rawDatas = ReadStrings(keys);
            progress?.Report(1);
            callback(rawDatas);
            yield break;
        }

        IEnumerator IReadableSerializableDataStorage.ReadBytesAsync(string[] keys, Action<byte[][]> callback, IProgress<float> progress)
        {
            var rawDatas = ReadBytes(keys);
            progress?.Report(1);
            callback(rawDatas);
            yield break;
        }

        IEnumerator IWritableSerializableDataStorage.WriteStringsAsync(string[] keys, string[] values, Action callback, IProgress<float> progress)
        {
            WriteStrings(keys, values);
            progress?.Report(1);
            callback?.Invoke();
            yield break;
        }

        IEnumerator IWritableSerializableDataStorage.WriteBytesAsync(string[] keys, byte[][] values, Action callback, IProgress<float> progress)
        {
            WriteBytes(keys, values);
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

        private static string[] ReadStrings(string[] keys)
        {
            return keys.Select(PlayerPrefs.GetString).ToArray();
        }

        private static byte[][] ReadBytes(string[] keys)
        {
            return keys.Select(PlayerPrefs.GetString).Select(Convert.FromBase64String).ToArray();
        }

        private static void WriteStrings(string[] keys, string[] values)
        {
            IterTools.StrictZip(keys, values).ForEach(PlayerPrefs.SetString);
        }

        private static void WriteBytes(string[] keys, byte[][] values)
        {
            IterTools.StrictZip(keys, values.Select(Convert.ToBase64String)).ForEach(PlayerPrefs.SetString);
        }

        private static void Flush()
        {
            PlayerPrefs.Save();
        }
    }
}