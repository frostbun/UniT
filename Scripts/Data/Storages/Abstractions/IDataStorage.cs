#nullable enable
namespace UniT.Data.Storage
{
    using System;

    public interface IDataStorage
    {
        public bool CanStore(Type type);
    }
}