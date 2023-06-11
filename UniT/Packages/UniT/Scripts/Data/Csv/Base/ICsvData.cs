namespace UniT.Data.Csv.Base
{
    using System;
    using UniT.Data.Base;

    public interface ICsvData : IData
    {
        protected internal bool Add(object row);

        protected internal Type GetRowType();
    }
}