namespace UniT.Data.Csv.Base
{
    using System;
    using System.Collections.Generic;

    public abstract class BaseCsvData<T> : List<T>, ICsvData
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

    public abstract class BaseCsvData<TKey, TValue> : Dictionary<TKey, TValue>, ICsvData
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