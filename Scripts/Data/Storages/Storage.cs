namespace UniT.Data.Storages
{
    using System;
    using System.Threading;
    #if UNIT_UNITASK
    using Cysharp.Threading.Tasks;
    #endif

    public abstract class Storage : IStorage
    {
        bool IStorage.CanStore(Type type) => this.CanStore(type);

        string[] IStorage.Load(string[] keys) => this.Load(keys);

        protected abstract bool CanStore(Type type);

        protected abstract string[] Load(string[] keys);

        #if UNIT_UNITASK
        UniTask<string[]> IStorage.LoadAsync(string[] keys, IProgress<float> progress, CancellationToken cancellationToken) => this.LoadAsync(keys, progress, cancellationToken);

        protected abstract UniTask<string[]> LoadAsync(string[] keys, IProgress<float> progress, CancellationToken cancellationToken);
        #endif
    }
}