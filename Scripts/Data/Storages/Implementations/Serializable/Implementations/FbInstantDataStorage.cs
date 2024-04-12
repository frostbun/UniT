#if UNIT_FBINSTANT
namespace UniT.Data
{
    using System;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniT.Extensions;
    using UniT.FbInstant;
    #else
    using System.Collections;
    #endif

    public sealed class FbInstantDataStorage : SerializableDataStorage, IReadableSerializableDataStorage, IWritableSerializableDataStorage
    {
        [Preserve]
        public FbInstantDataStorage()
        {
        }

        protected override bool CanStore(Type type) => base.CanStore(type)
            && typeof(IReadableData).IsAssignableFrom(type)
            && typeof(IWritableData).IsAssignableFrom(type);

        string[] IReadableSerializableDataStorage.Load(string[] keys)
        {
            throw new NotSupportedException("FbInstant only supports async operations. Please use LoadAsync instead.");
        }

        void IWritableSerializableDataStorage.Save(string[] keys, string[] rawDatas)
        {
            throw new NotSupportedException("FbInstant only supports async operations. Please use SaveAsync instead.");
        }

        void IWritableDataStorage.Flush()
        {
            throw new NotSupportedException("FbInstant only supports async operations. Please use FlushAsync instead.");
        }

        #if UNIT_UNITASK
        UniTask<string[]> IReadableSerializableDataStorage.LoadAsync(string[] keys, IProgress<float> progress, CancellationToken _)
        {
            return FbInstant.Player.LoadData(keys).ContinueWith(result =>
            {
                if (result.IsError) throw new($"Load {keys.ToArrayString()} error: {result.Error}");
                progress?.Report(1);
                return result.Data;
            });
        }

        UniTask IWritableSerializableDataStorage.SaveAsync(string[] keys, string[] rawDatas, IProgress<float> progress, CancellationToken _)
        {
            return FbInstant.Player.SaveData(keys, rawDatas).ContinueWith(result =>
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
        IEnumerator IReadableSerializableDataStorage.LoadAsync(string[] keys, Action<string[]> callback, IProgress<float> progress)
        {
            throw new NotImplementedException();
        }

        IEnumerator IWritableSerializableDataStorage.SaveAsync(string[] keys, string[] rawDatas, Action callback, IProgress<float> progress)
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