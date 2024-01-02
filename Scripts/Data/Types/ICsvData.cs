namespace UniT.Data.Types
{
    using System;

    public interface ICsvData : ISerializableData
    {
        public Type RowType { get; }

        public void Add(object key, object value);
    }
}