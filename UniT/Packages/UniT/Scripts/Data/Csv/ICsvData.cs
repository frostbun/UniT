namespace UniT.Data.Csv
{
    using System;

    public interface ICsvData : IData
    {
        protected internal Type RowType { get; }

        protected internal void Add(object key, object value);
    }
}