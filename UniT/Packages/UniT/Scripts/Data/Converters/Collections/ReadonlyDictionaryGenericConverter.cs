namespace UniT.Data.Converters.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UniT.Data.Converters.Base;

    /// <summary>
    ///     Depends on <see cref="DictionaryGenericConverter"/>
    /// </summary>
    public class ReadonlyDictionaryGenericConverter : BaseGenericConverter
    {
        protected override Type ConvertibleType => typeof(ReadOnlyDictionary<,>);

        protected override object ConvertFromString(string str, Type type)
        {
            return Activator.CreateInstance(type, ConverterManager.Instance.ConvertFromString(str, typeof(Dictionary<,>).MakeGenericType(type.GetGenericArguments())));
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ConverterManager.Instance.ConvertToString(obj, typeof(Dictionary<,>).MakeGenericType(type.GetGenericArguments()));
        }
    }
}