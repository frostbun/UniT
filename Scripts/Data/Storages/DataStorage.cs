namespace UniT.Data.Storages
{
    using System;

    public abstract class DataStorage : IDataStorage
    {
        bool IDataStorage.CanStore(Type type) => this.CanStore(type);

        protected abstract bool CanStore(Type type);
    }
}