namespace UniT.Data.Converters.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UniT.Data.Converters.Base;

    public class ReadonlyDictionaryGenericConverter : BaseGenericConverter
    {
        protected override Type ConvertibleType => typeof(ReadOnlyDictionary<,>);

        protected override object ConvertFromString_Internal(string str, Type type)
        {
            var dictionaryType      = typeof(Dictionary<,>).MakeGenericType(type.GetGenericArguments());
            var dictionaryConverter = ConverterManager.Instance.GetConverter(dictionaryType);
            return Activator.CreateInstance(type, dictionaryConverter.ConvertFromString(str, dictionaryType));
        }

        protected override string ConvertToString_Internal(object obj, Type type)
        {
            var dictionaryType      = typeof(Dictionary<,>).MakeGenericType(type.GetGenericArguments());
            var dictionaryConverter = ConverterManager.Instance.GetConverter(dictionaryType);
            return dictionaryConverter.ConvertToString(obj, dictionaryType);
        }
    }
}