namespace UniT.Data
{
    using System;

    public abstract class ReadOnlyNonSerializableDataStorage : NonSerializableDataStorage, IReadOnlyNonSerializableDataStorage
    {
        protected override bool CanStore(Type type) => base.CanStore(type) && typeof(IReadOnlyData).IsAssignableFrom(type);
    }
}