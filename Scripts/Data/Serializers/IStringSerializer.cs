namespace UniT.Data
{
    public interface IStringSerializer : ISerializer
    {
        public void Populate(IData data, string rawData);

        public string Serialize(IData data);
    }
}