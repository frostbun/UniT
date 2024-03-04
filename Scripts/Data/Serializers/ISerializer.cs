namespace UniT.Data
{
    using System;

    public interface ISerializer
    {
        public bool CanSerialize(Type type);

        public void Populate(IData data, string rawData);

        public string Serialize(IData data);
    }
}