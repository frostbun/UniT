namespace UniT.Data.Converters.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    ///     Depends on <see cref="ListConverter"/>
    /// </summary>
    public sealed class ReadOnlyCollectionConverter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(ReadOnlyCollection<>);

        protected override object ConvertFromString(string str, Type type)
        {
            return Activator.CreateInstance(type, ConverterManager.Instance.ConvertFromString(str, MakeListType(type)));
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ConverterManager.Instance.ConvertToString(obj, MakeListType(type));
        }

        private static Type MakeListType(Type type)
        {
            return typeof(List<>).MakeGenericType(type.GetGenericArguments());
        }
    }
}