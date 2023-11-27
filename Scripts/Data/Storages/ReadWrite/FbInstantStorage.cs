#if UNIT_FBINSTANT
namespace UniT.Data.Storages
{
    using System;
    using UniT.Extensions;
    using UniT.FbInstant;
    using UnityEngine.Scripting;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

    public sealed class FbInstantStorage : ReadWriteStorage
    {
        [Preserve]
        public FbInstantStorage()
        {
        }

        protected override string[] Load(string[] keys)
        {
            throw new NotSupportedException("FbInstant only supports async methods. Please install UniTask and use LoadAsync instead.");
        }

        protected override void Save(string[] keys, string[] rawDatas)
        {
            throw new NotSupportedException("FbInstant only supports async methods. Please install UniTask and use SaveAsync instead.");
        }

        protected override void Flush()
        {
            throw new NotSupportedException("FbInstant only supports async methods. Please install UniTask and use FlushAsync instead.");
        }

        #if UNIT_UNITASK
        protected override UniTask<string[]> LoadAsync(string[] keys, IProgress<float> progress, CancellationToken _)
        {
            return FbInstant.Player.LoadData(keys).ContinueWith(result =>
            {
                if (result.IsError) throw new($"Load {keys.ToArrayString()} error: {result.Error}");
                progress?.Report(1);
                return result.Data;
            });
        }

        protected override UniTask SaveAsync(string[] keys, string[] rawDatas, IProgress<float> progress, CancellationToken _)
        {
            return FbInstant.Player.SaveData(keys, rawDatas).ContinueWith(result =>
            {
                if (result.IsError) throw new($"Save {keys.ToArrayString()} error: {result.Error}");
                progress?.Report(1);
            });
        }

        protected override UniTask FlushAsync(IProgress<float> progress, CancellationToken _)
        {
            return FbInstant.Player.FlushData().ContinueWith(result =>
            {
                if (result.IsError) throw new($"Flush {this.GetType().Name} error: {result.Error}");
                progress?.Report(1);
            });
        }
        #endif
    }
}
#endif