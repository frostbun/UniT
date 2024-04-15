namespace UniT.Data
{
    public interface IBinarySerializer : ISerializer
    {
        public void Populate(IData data, byte[] rawData);

        public byte[] Serialize(IData data);
    }
}