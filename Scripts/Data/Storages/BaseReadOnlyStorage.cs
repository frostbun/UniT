namespace UniT.Data.Storages
{
    using System;
    using Cysharp.Threading.Tasks;

    public abstract class BaseReadOnlyStorage : IReadOnlyStorage
    {
        bool IReadOnlyStorage.CanStore(Type type) => this.CanStore(type);

        UniTask<string[]> IReadOnlyStorage.Load(string[] keys) => this.Load(keys);

        protected virtual bool CanStore(Type type) =>
            typeof(IReadOnlyData).IsAssignableFrom(type)
            && !typeof(IData).IsAssignableFrom(type);

        protected abstract UniTask<string[]> Load(string[] keys);
    }
}