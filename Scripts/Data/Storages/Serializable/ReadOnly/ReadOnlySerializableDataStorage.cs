namespace UniT.Data
{
    using System;

    public abstract class ReadOnlySerializableDataStorage : SerializableDataStorage, IReadOnlySerializableDataStorage
    {
        protected override bool CanStore(Type type) => base.CanStore(type) && typeof(IReadOnlyData).IsAssignableFrom(type);
    }
}