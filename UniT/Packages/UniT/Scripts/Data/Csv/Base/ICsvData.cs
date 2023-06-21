namespace UniT.Data.Csv.Base
{
    using System;
    using UniT.Data.Base;

    public interface ICsvData : IData
    {
        protected internal Type GetRowType();

        protected internal void Add(object key, object value);
    }
}