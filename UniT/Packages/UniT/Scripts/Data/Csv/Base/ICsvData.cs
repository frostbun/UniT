namespace UniT.Data.Csv.Base
{
    using System;
    using UniT.Data.Base;

    public interface ICsvData : IData
    {
        public bool AddRow(object row);

        public Type GetRowType();
    }
}