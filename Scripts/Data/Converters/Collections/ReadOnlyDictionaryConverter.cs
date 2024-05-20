namespace UniT.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UniT.Extensions;

    /// <summary>
    ///     Depends on <see cref="DictionaryConverter"/>
    /// </summary>
    public sealed class ReadOnlyDictionaryConverter : Converter
    {
        protected override bool CanConvert(Type type) => type.DerivesFrom(typeof(ReadOnlyDictionary<,>));

        protected override object ConvertFromString(string str, Type type)
        {
            return Activator.CreateInstance(type, ConverterManager.ConvertFromString(str, MakeDictionaryType(type)));
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ConverterManager.ConvertToString(obj, MakeDictionaryType(type));
        }

        private static Type MakeDictionaryType(Type type)
        {
            return typeof(Dictionary<,>).MakeGenericType(type.GetGenericArguments());
        }
    }
}