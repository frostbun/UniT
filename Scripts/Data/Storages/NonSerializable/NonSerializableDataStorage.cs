namespace UniT.Data.Storages
{
    using System;
    using UniT.Data.Types;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #endif

    public abstract class NonSerializableDataStorage : DataStorage, INonSerializableDataStorage
    {
        protected override bool CanStore(Type type) => typeof(INonSerializableData).IsAssignableFrom(type);

        IData[] INonSerializableDataStorage.Load(string[] keys) => this.Load(keys);

        protected abstract IData[] Load(string[] keys);

        #if UNIT_UNITASK
        UniTask<IData[]> INonSerializableDataStorage.LoadAsync(string[] keys, IProgress<float> progress, CancellationToken cancellationToken) => this.LoadAsync(keys, progress, cancellationToken);

        protected abstract UniTask<IData[]> LoadAsync(string[] keys, IProgress<float> progress, CancellationToken cancellationToken);
        #endif
    }
}