#if UNIT_FBINSTANT
#nullable enable
namespace UniT.Data
{
    using System;
    using System.Linq;
    using UniT.FbInstant;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    using UniT.Extensions;
    #endif

    public sealed class FbInstantDataStorage : IReadableSerializableDataStorage, IWritableSerializableDataStorage
    {
        [Preserve]
        public FbInstantDataStorage()
        {
        }

        bool IDataStorage.CanStore(Type type) => typeof(ISerializableData).IsAssignableFrom(type)
            && typeof(IReadableData).IsAssignableFrom(type)
            && typeof(IWritableData).IsAssignableFrom(type);

        string[] IReadableSerializableDataStorage.ReadStrings(string[] keys)
        {
            throw new NotSupportedException("FbInstant only supports async operations. Please use LoadAsync instead.");
        }

        byte[][] IReadableSerializableDataStorage.ReadBytes(string[] keys)
        {
            throw new NotSupportedException("FbInstant only supports async operations. Please use LoadAsync instead.");
        }

        void IWritableSerializableDataStorage.WriteStrings(string[] keys, string[] rawDatas)
        {
            throw new NotSupportedException("FbInstant only supports async operations. Please use SaveAsync instead.");
        }

        void IWritableSerializableDataStorage.WriteBytes(string[] keys, byte[][] rawDatas)
        {
            throw new NotSupportedException("FbInstant only supports async operations. Please use SaveAsync instead.");
        }

        void IWritableDataStorage.Flush()
        {
            throw new NotSupportedException("FbInstant only supports async operations. Please use FlushAsync instead.");
        }

        #if UNIT_UNITASK
        async UniTask<string[]> IReadableSerializableDataStorage.ReadStringsAsync(string[] keys, IProgress<float>? progress, CancellationToken _)
        {
            var texts = await FbInstant.Player.LoadDataAsync(keys);
            progress?.Report(1);
            return texts;
        }

        async UniTask<byte[][]> IReadableSerializableDataStorage.ReadBytesAsync(string[] keys, IProgress<float>? progress, CancellationToken _)
        {
            var texts = await FbInstant.Player.LoadDataAsync(keys);
            progress?.Report(1);
            return texts.Select(Convert.FromBase64String).ToArray();
        }

        async UniTask IWritableSerializableDataStorage.WriteStringsAsync(string[] keys, string[] rawDatas, IProgress<float>? progress, CancellationToken _)
        {
            await FbInstant.Player.SaveDataAsync(keys, rawDatas);
            progress?.Report(1);
        }

        async UniTask IWritableSerializableDataStorage.WriteBytesAsync(string[] keys, byte[][] rawDatas, IProgress<float>? progress, CancellationToken _)
        {
            await FbInstant.Player.SaveDataAsync(keys, rawDatas.Select(Convert.ToBase64String).ToArray());
            progress?.Report(1);
        }

        async UniTask IWritableDataStorage.FlushAsync(IProgress<float>? progress, CancellationToken _)
        {
            await FbInstant.Player.FlushDataAsync();
            progress?.Report(1);
        }
        #else
        IEnumerator IReadableSerializableDataStorage.ReadStringsAsync(string[] keys, Action<string[]> callback, IProgress<float>? progress)
        {
            return FbInstant.Player.LoadDataAsync(keys).ToCoroutine(texts =>
            {
                progress?.Report(1);
                callback(texts);
            });
        }

        IEnumerator IReadableSerializableDataStorage.ReadBytesAsync(string[] keys, Action<byte[][]> callback, IProgress<float>? progress)
        {
            return FbInstant.Player.LoadDataAsync(keys).ToCoroutine(texts =>
            {
                progress?.Report(1);
                callback(texts.Select(Convert.FromBase64String).ToArray());
            });
        }

        IEnumerator IWritableSerializableDataStorage.WriteStringsAsync(string[] keys, string[] rawDatas, Action? callback, IProgress<float>? progress)
        {
            return FbInstant.Player.SaveDataAsync(keys, rawDatas).ToCoroutine(() =>
            {
                progress?.Report(1);
                callback?.Invoke();
            });
        }

        IEnumerator IWritableSerializableDataStorage.WriteBytesAsync(string[] keys, byte[][] rawDatas, Action? callback, IProgress<float>? progress)
        {
            return FbInstant.Player.SaveDataAsync(keys, rawDatas.Select(Convert.ToBase64String).ToArray()).ToCoroutine(() =>
            {
                progress?.Report(1);
                callback?.Invoke();
            });
        }

        IEnumerator IWritableDataStorage.FlushAsync(Action? callback, IProgress<float>? progress)
        {
            return FbInstant.Player.FlushDataAsync().ToCoroutine(() =>
            {
                progress?.Report(1);
                callback?.Invoke();
            });
        }
        #endif
    }
}
#endif