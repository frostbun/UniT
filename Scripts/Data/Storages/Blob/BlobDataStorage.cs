namespace UniT.Data.Storages
{
    using System;
    using UniT.Data.Types;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

    public abstract class BlobDataStorage : DataStorage, IBlobDataStorage
    {
        protected override bool CanStore(Type type) => typeof(IBlobData).IsAssignableFrom(type);

        IData[] IBlobDataStorage.Load(string[] keys) => this.Load(keys);

        protected abstract IData[] Load(string[] keys);

        #if UNIT_UNITASK
        UniTask<IData[]> IBlobDataStorage.LoadAsync(string[] keys, IProgress<float> progress, CancellationToken cancellationToken) => this.LoadAsync(keys, progress, cancellationToken);

        protected abstract UniTask<IData[]> LoadAsync(string[] keys, IProgress<float> progress, CancellationToken cancellationToken);
        #endif
    }
}