namespace UniT.Data.Csv.Base
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UniT.Extensions;

    public abstract class BaseCsvData<T> : List<T>, ICsvData
    {
        private readonly MemberInfo keyMember;

        protected BaseCsvData()
        {
            var key = typeof(T).GetCsvKeyAttribute();
            this.keyMember = typeof(T).GetAllFieldsOrProperties().First(field => field.Name == key);
        }

        bool ICsvData.Add(object row)
        {
            var keyValue = this.keyMember switch
            {
                FieldInfo keyField       => keyField.GetValue(row),
                PropertyInfo keyProperty => keyProperty.GetValue(row),
                _                        => null,
            };
            if (keyValue is null) return false;
            this.Add((T)row);
            return true;
        }

        Type ICsvData.GetRowType()
        {
            return typeof(T);
        }
    }

    public abstract class BaseCsvData<TKey, TValue> : Dictionary<TKey, TValue>, ICsvData
    {
        private readonly MemberInfo keyMember;

        protected BaseCsvData()
        {
            var key = typeof(TValue).GetCsvKeyAttribute();
            this.keyMember = typeof(TValue).GetAllFieldsOrProperties().First(field => field.Name == key);
        }

        bool ICsvData.Add(object row)
        {
            var keyValue = this.keyMember switch
            {
                FieldInfo keyField       => keyField.GetValue(row),
                PropertyInfo keyProperty => keyProperty.GetValue(row),
                _                        => null,
            };
            if (keyValue is null) return false;
            this.Add((TKey)keyValue, (TValue)row);
            return true;
        }

        Type ICsvData.GetRowType()
        {
            return typeof(TValue);
        }
    }
}