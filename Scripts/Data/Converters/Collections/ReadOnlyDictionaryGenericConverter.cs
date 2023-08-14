namespace UniT.Data.Converters.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    ///     Depends on <see cref="DictionaryGenericConverter"/>
    /// </summary>
    public class ReadOnlyDictionaryGenericConverter : BaseGenericConverter
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Type MakeDictionaryType(Type type)
        {
            return typeof(Dictionary<,>).MakeGenericType(type.GetGenericArguments());
        }
    }
}