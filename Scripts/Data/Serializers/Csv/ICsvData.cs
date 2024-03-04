namespace UniT.Data
{
    using System;

    public interface ICsvData : ISerializableData
    {
        public string Key { get; }

        public string Prefix { get; }

        public Type RowType { get; }

        public void Add(object key, object value);
    }
}