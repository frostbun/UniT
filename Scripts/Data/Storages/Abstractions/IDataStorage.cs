namespace UniT.Data
{
    using System;

    public interface IDataStorage
    {
        public bool CanStore(Type type);
    }
}