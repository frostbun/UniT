namespace UniT.Data.Serializers
{
    using System;

    public interface ICsvData
    {
        public Type RowType { get; }

        public void Add(object key, object value);
    }
}