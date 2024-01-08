namespace UniT.Data.Storages
{
    using System;
    using UniT.Data.Types;

    public abstract class ReadOnlyNonSerializableDataStorage : NonSerializableDataStorage, IReadOnlyNonSerializableDataStorage
    {
        protected override bool CanStore(Type type) => base.CanStore(type) && typeof(IReadOnlyData).IsAssignableFrom(type);
    }
}