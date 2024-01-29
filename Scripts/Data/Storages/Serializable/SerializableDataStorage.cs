namespace UniT.Data.Storages
{
    using System;
    using UniT.Data.Types;
    #if UNIT_UNITASK
    using System.Threading;
    using Cysharp.Threading.Tasks;
    #else
    using System.Collections;
    #endif

    public abstract class SerializableDataStorage : DataStorage, ISerializableDataStorage
    {
        protected override bool CanStore(Type type) => typeof(ISerializableData).IsAssignableFrom(type);

        string[] ISerializableDataStorage.Load(string[] keys) => this.Load(keys);

        protected abstract string[] Load(string[] keys);

        #if UNIT_UNITASK
        UniTask<string[]> ISerializableDataStorage.LoadAsync(string[] keys, IProgress<float> progress, CancellationToken cancellationToken) => this.LoadAsync(keys, progress, cancellationToken);

        protected abstract UniTask<string[]> LoadAsync(string[] keys, IProgress<float> progress, CancellationToken cancellationToken);
        #else
        IEnumerator ISerializableDataStorage.LoadAsync(string[] keys, Action<string[]> callback, IProgress<float> progress) => this.LoadAsync(keys, callback, progress);

        protected abstract IEnumerator LoadAsync(string[] keys, Action<string[]> callback, IProgress<float> progress);
        #endif
    }
}