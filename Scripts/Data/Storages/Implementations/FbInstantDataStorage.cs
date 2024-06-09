#if UNIT_FBINSTANT
#nullable enable
namespace UniT.Data.Storage
{
    using System;
    using UniT.FbInstant;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    using UniT.Extensions;
    #endif

    public sealed class FbInstantDataStorage : IReadableStringStorage, IWritableStringStorage
    {
        [Preserve]
        public FbInstantDataStorage()
        {
        }

        bool IDataStorage.CanStore(Type type) => typeof(IReadableData).IsAssignableFrom(type) && typeof(IWritableData).IsAssignableFrom(type);

        string[] IReadableStringStorage.Read(string[] keys)
        {
            throw new NotSupportedException("FbInstant only supports async operations. Please use LoadAsync instead.");
        }

        void IWritableStringStorage.Write(string[] keys, string[] rawDatas)
        {
            throw new NotSupportedException("FbInstant only supports async operations. Please use SaveAsync instead.");
        }

        void IWritableDataStorage.Flush()
        {
            throw new NotSupportedException("FbInstant only supports async operations. Please use FlushAsync instead.");
        }

        #if UNIT_UNITASK
        async UniTask<string[]> IReadableStringStorage.ReadAsync(string[] keys, IProgress<float>? progress, CancellationToken _)
        {
            var texts = await FbInstant.Player.LoadDataAsync(keys);
            progress?.Report(1);
            return texts;
        }

        async UniTask IWritableStringStorage.WriteAsync(string[] keys, string[] rawDatas, IProgress<float>? progress, CancellationToken _)
        {
            await FbInstant.Player.SaveDataAsync(keys, rawDatas);
            progress?.Report(1);
        }

        async UniTask IWritableDataStorage.FlushAsync(IProgress<float>? progress, CancellationToken _)
        {
            await FbInstant.Player.FlushDataAsync();
            progress?.Report(1);
        }
        #else
        IEnumerator IReadableStringStorage.ReadAsync(string[] keys, Action<string[]> callback, IProgress<float>? progress)
        {
            return FbInstant.Player.LoadDataAsync(keys).ToCoroutine(texts =>
            {
                progress?.Report(1);
                callback(texts);
            });
        }

        IEnumerator IWritableStringStorage.WriteAsync(string[] keys, string[] rawDatas, Action? callback, IProgress<float>? progress)
        {
            return FbInstant.Player.SaveDataAsync(keys, rawDatas).ToCoroutine(() =>
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