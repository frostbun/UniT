namespace UniT.Data.Types
{
    using System;

    public interface ICsvData : ISerializableData
    {
        public string Prefix { get; }

        public Type RowType { get; }

        public void Add(object key, object value);
    }
}