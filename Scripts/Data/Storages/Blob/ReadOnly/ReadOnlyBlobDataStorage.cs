namespace UniT.Data.Storages
{
    using System;
    using UniT.Data.Types;

    public abstract class ReadOnlyBlobDataStorage : BlobDataStorage, IReadOnlyBlobDataStorage
    {
        protected override bool CanStore(Type type) => base.CanStore(type) && typeof(IReadOnlyData).IsAssignableFrom(type);
    }
}