namespace UniT.Data
{
    using System;

    public abstract class SerializableDataStorage : DataStorage
    {
        protected override bool CanStore(Type type) => typeof(ISerializableData).IsAssignableFrom(type);
    }
}