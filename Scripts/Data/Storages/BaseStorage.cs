namespace UniT.Data.Storages
{
    using System;
    using Cysharp.Threading.Tasks;

    public abstract class BaseStorage : BaseReadOnlyStorage, IStorage
    {
        UniTask IStorage.Save(string[] keys, string[] values) => this.Save(keys, values);

        UniTask IStorage.Flush() => this.Flush();

        protected override bool CanStore(Type type) => typeof(IData).IsAssignableFrom(type);

        protected abstract UniTask Save(string[] keys, string[] values);

        protected abstract UniTask Flush();
    }
}