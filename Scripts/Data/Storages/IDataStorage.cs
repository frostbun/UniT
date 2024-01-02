namespace UniT.Data.Storages
{
    using System;

    public interface IDataStorage
    {
        public bool CanStore(Type type);
    }
}