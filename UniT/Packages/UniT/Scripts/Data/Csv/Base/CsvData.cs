namespace UniT.Data.Csv.Base
{
    using System;
    using System.Collections.Generic;
    using UniT.Extensions;

    public class CsvData : ICsvData
    {
        Type ICsvData.GetRowType()
        {
            return this.GetType();
        }

        void ICsvData.Add(object key, object value)
        {
            value.CopyTo(this);
        }
    }

    public class CsvData<T> : List<T>, ICsvData
    {
        Type ICsvData.GetRowType()
        {
            return typeof(T);
        }

        void ICsvData.Add(object key, object value)
        {
            this.Add((T)value);
        }
    }

    public class CsvData<TKey, TValue> : Dictionary<TKey, TValue>, ICsvData
    {
        Type ICsvData.GetRowType()
        {
            return typeof(TValue);
        }

        void ICsvData.Add(object key, object value)
        {
            this.Add((TKey)key, (TValue)value);
        }
    }
}