namespace UniT.Data.Csv.Base
{
    using System;
    using UniT.Data.Base;

    public interface ICsvData : IData
    {
        protected internal void Add(object key, object value);

        protected internal Type GetRowType();
    }
}