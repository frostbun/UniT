namespace UniT.Data
{
    using System;

    public abstract class NonSerializableDataStorage : DataStorage
    {
        protected override bool CanStore(Type type) => typeof(INonSerializableData).IsAssignableFrom(type);
    }
}