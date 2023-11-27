namespace UniT.Data.Storages
{
    using System;

    public abstract class ReadOnlyStorage : Storage, IReadOnlyStorage
    {
        protected override bool CanStore(Type type) => typeof(IReadOnlyData).IsAssignableFrom(type);
    }
}