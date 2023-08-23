namespace UniT.Data.Csv
{
    using System;

    public interface ICsvData : IData
    {
        public Type RowType { get; }

        public void Add(object key, object value);
    }
}