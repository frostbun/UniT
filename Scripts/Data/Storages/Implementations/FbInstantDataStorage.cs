#if UNIT_FBINSTANT
namespace UniT.Data
{
    using System;
    using System.Linq;
    using UniT.Extensions;
    using UniT.FbInstant;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
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
        async UniTask<string[]> IReadableSerializableDataStorage.ReadStringsAsync(string[] keys, IProgress<float> progress, CancellationToken _)
        {
            var result = await FbInstant.Player.LoadDataAsync(keys);
            if (result.IsError) throw new($"Load {keys.ToArrayString()} error: {result.Error}");
            progress?.Report(1);
            return result.Data;
        }

        async UniTask<byte[][]> IReadableSerializableDataStorage.ReadBytesAsync(string[] keys, IProgress<float> progress, CancellationToken _)
        {
            var result = await FbInstant.Player.LoadDataAsync(keys);
            if (result.IsError) throw new($"Load {keys.ToArrayString()} error: {result.Error}");
            progress?.Report(1);
            return result.Data.Select(Convert.FromBase64String).ToArray();
        }

        async UniTask IWritableSerializableDataStorage.WriteStringsAsync(string[] keys, string[] rawDatas, IProgress<float> progress, CancellationToken _)
        {
            var result = await FbInstant.Player.SaveDataAsync(keys, rawDatas);
            if (result.IsError) throw new($"Save {keys.ToArrayString()} error: {result.Error}");
            progress?.Report(1);
        }

        async UniTask IWritableSerializableDataStorage.WriteBytesAsync(string[] keys, byte[][] rawDatas, IProgress<float> progress, CancellationToken _)
        {
            var result = await FbInstant.Player.SaveDataAsync(keys, rawDatas.Select(Convert.ToBase64String).ToArray());
            if (result.IsError) throw new($"Save {keys.ToArrayString()} error: {result.Error}");
            progress?.Report(1);
        }

        async UniTask IWritableDataStorage.FlushAsync(IProgress<float> progress, CancellationToken _)
        {
            var result = await FbInstant.Player.FlushDataAsync();
            if (result.IsError) throw new($"Flush {this.GetType().Name} error: {result.Error}");
            progress?.Report(1);
        }
        #else
        IEnumerator IReadableSerializableDataStorage.ReadStringsAsync(string[] keys, Action<string[]> callback, IProgress<float> progress)
        {
            return FbInstant.Player.LoadDataAsync(keys).ToCoroutine(result =>
            {
                if (result.IsError) throw new($"Load {keys.ToArrayString()} error: {result.Error}");
                progress?.Report(1);
                callback(result.Data);
            });
        }

        IEnumerator IReadableSerializableDataStorage.ReadBytesAsync(string[] keys, Action<byte[][]> callback, IProgress<float> progress)
        {
            return FbInstant.Player.LoadDataAsync(keys).ToCoroutine(result =>
            {
                if (result.IsError) throw new($"Load {keys.ToArrayString()} error: {result.Error}");
                progress?.Report(1);
                callback(result.Data.Select(Convert.FromBase64String).ToArray());
            });
        }

        IEnumerator IWritableSerializableDataStorage.WriteStringsAsync(string[] keys, string[] rawDatas, Action callback, IProgress<float> progress)
        {
            return FbInstant.Player.SaveDataAsync(keys, rawDatas).ToCoroutine(result =>
            {
                if (result.IsError) throw new($"Save {keys.ToArrayString()} error: {result.Error}");
                progress?.Report(1);
                callback?.Invoke();
            });
        }

        IEnumerator IWritableSerializableDataStorage.WriteBytesAsync(string[] keys, byte[][] rawDatas, Action callback, IProgress<float> progress)
        {
            return FbInstant.Player.SaveDataAsync(keys, rawDatas.Select(Convert.ToBase64String).ToArray()).ToCoroutine(result =>
            {
                if (result.IsError) throw new($"Save {keys.ToArrayString()} error: {result.Error}");
                progress?.Report(1);
                callback?.Invoke();
            });
        }

        IEnumerator IWritableDataStorage.FlushAsync(Action callback, IProgress<float> progress)
        {
            return FbInstant.Player.FlushDataAsync().ToCoroutine(result =>
            {
                if (result.IsError) throw new($"Flush {this.GetType().Name} error: {result.Error}");
                progress?.Report(1);
                callback?.Invoke();
            });
        }
        #endif
    }
}
#endif