#if UNIT_FBINSTANT
namespace UniT.Data
{
    using System;
    using System.Linq;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using UniT.FbInstant;
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
        UniTask<string[]> IReadableSerializableDataStorage.ReadStringsAsync(string[] keys, IProgress<float> progress, CancellationToken _)
        {
            return FbInstant.Player.LoadData(keys).ContinueWith(result =>
            {
                if (result.IsError) throw new($"Load {keys.ToArrayString()} error: {result.Error}");
                progress?.Report(1);
                return result.Data;
            });
        }

        UniTask<byte[][]> IReadableSerializableDataStorage.ReadBytesAsync(string[] keys, IProgress<float> progress, CancellationToken _)
        {
            return FbInstant.Player.LoadData(keys).ContinueWith(result =>
            {
                if (result.IsError) throw new($"Load {keys.ToArrayString()} error: {result.Error}");
                progress?.Report(1);
                return result.Data.Select(Convert.FromBase64String).ToArray();
            });
        }

        UniTask IWritableSerializableDataStorage.WriteStringsAsync(string[] keys, string[] rawDatas, IProgress<float> progress, CancellationToken _)
        {
            return FbInstant.Player.SaveData(keys, rawDatas).ContinueWith(result =>
            {
                if (result.IsError) throw new($"Save {keys.ToArrayString()} error: {result.Error}");
                progress?.Report(1);
            });
        }

        UniTask IWritableSerializableDataStorage.WriteBytesAsync(string[] keys, byte[][] rawDatas, IProgress<float> progress, CancellationToken _)
        {
            return FbInstant.Player.SaveData(keys, rawDatas.Select(Convert.ToBase64String).ToArray()).ContinueWith(result =>
            {
                if (result.IsError) throw new($"Save {keys.ToArrayString()} error: {result.Error}");
                progress?.Report(1);
            });
        }

        UniTask IWritableDataStorage.FlushAsync(IProgress<float> progress, CancellationToken _)
        {
            return FbInstant.Player.FlushData().ContinueWith(result =>
            {
                if (result.IsError) throw new($"Flush {this.GetType().Name} error: {result.Error}");
                progress?.Report(1);
            });
        }
        #else
        IEnumerator IReadableSerializableDataStorage.ReadStringsAsync(string[] keys, Action<string[]> callback, IProgress<float> progress)
        {
            throw new NotImplementedException();
        }

        IEnumerator IReadableSerializableDataStorage.ReadBytesAsync(string[] keys, Action<byte[][]> callback, IProgress<float> progress)
        {
            throw new NotImplementedException();
        }

        IEnumerator IWritableSerializableDataStorage.WriteStringsAsync(string[] keys, string[] rawDatas, Action callback, IProgress<float> progress)
        {
            throw new NotImplementedException();
        }

        IEnumerator IWritableSerializableDataStorage.WriteBytesAsync(string[] keys, byte[][] rawDatas, Action callback, IProgress<float> progress)
        {
            throw new NotImplementedException();
        }

        IEnumerator IWritableDataStorage.FlushAsync(Action callback, IProgress<float> progress)
        {
            throw new NotImplementedException();
        }
        #endif
    }
}
#endif