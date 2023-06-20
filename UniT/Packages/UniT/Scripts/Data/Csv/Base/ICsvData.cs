namespace UniT.Data.Csv.Base
{
    using System;
    using UniT.Data.Base;

    public interface ICsvData : IData
    {
        public Type GetRowType();

        public void Add(object key, object value);
    }
}