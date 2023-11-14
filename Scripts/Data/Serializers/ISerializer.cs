namespace UniT.Data.Serializers
{
    using System;

    public interface ISerializer
    {
        public bool CanSerialize(Type type);

        public void Populate(object data, string rawData);

        public string Serialize(object data);
    }
}