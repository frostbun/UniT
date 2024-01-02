namespace UniT.Data.Storages
{
    using System;
    using UniT.Data.Types;

    public abstract class ReadOnlySerializableDataStorage : SerializableDataStorage, IReadOnlySerializableDataStorage
    {
        protected override bool CanStore(Type type) => base.CanStore(type) && typeof(IReadOnlyData).IsAssignableFrom(type);
    }
}