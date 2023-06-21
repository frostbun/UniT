namespace UniT.Data.Converters.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UniT.Data.Converters.Base;

    public class ReadonlyCollectionGenericConverter : BaseGenericConverter
    {
        protected override Type ConvertibleType => typeof(ReadOnlyCollection<>);

        protected override object ConvertFromString_Internal(string str, Type type)
        {
            var listType      = typeof(List<>).MakeGenericType(type.GetGenericArguments());
            var listConverter = ConverterManager.Instance.GetConverter(listType);
            return Activator.CreateInstance(type, listConverter.ConvertFromString(str, listType));
        }

        protected override string ConvertToString_Internal(object obj, Type type)
        {
            var listType      = typeof(List<>).MakeGenericType(type.GetGenericArguments());
            var listConverter = ConverterManager.Instance.GetConverter(listType);
            return listConverter.ConvertToString(obj, listType);
        }
    }
}