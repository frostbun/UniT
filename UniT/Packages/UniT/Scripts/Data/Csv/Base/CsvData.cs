namespace UniT.Data.Csv.Base
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;

    public class CsvData : ICsvData
    {
        void ICsvData.Add(object key, object value)
        {
            value.CopyTo(this);
        }

        Type ICsvData.GetRowType()
        {
            return this.GetType();
        }
    }

    public class CsvData<T> : List<T>, ICsvData
    {
        void ICsvData.Add(object key, object value)
        {
            this.Add((T)value);
        }

        Type ICsvData.GetRowType()
        {
            return typeof(T);
        }
    }

    public class CsvData<TKey, TValue> : Dictionary<TKey, TValue>, ICsvData
    {
        void ICsvData.Add(object key, object value)
        {
            this.Add((TKey)key, (TValue)value);
        }

        Type ICsvData.GetRowType()
        {
            return typeof(TValue);
        }
    }
}