namespace UniT.Data.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    ///     Depends on <see cref="DictionaryConverter"/>
    /// </summary>
    public sealed class ReadOnlyDictionaryConverter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(ReadOnlyDictionary<,>);

        protected override object ConvertFromString(string str, Type type)
        {
            return Activator.CreateInstance(type, ConverterManager.Instance.ConvertFromString(str, MakeDictionaryType(type)));
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ConverterManager.Instance.ConvertToString(obj, MakeDictionaryType(type));
        }

        private static Type MakeDictionaryType(Type type)
        {
            return typeof(Dictionary<,>).MakeGenericType(type.GetGenericArguments());
        }
    }
}