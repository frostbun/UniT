namespace UniT.Data.Serializers
{
    using System;
    using UniT.Data.Types;

    public interface ISerializer
    {
        public bool CanSerialize(Type type);

        public void Populate(IData data, string rawData);

        public string Serialize(IData data);
    }
}