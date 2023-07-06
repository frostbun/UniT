namespace UniT.Data.Converters.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using UniT.Data.Converters.Base;

    /// <summary>
    ///     Depends on <see cref="ListGenericConverter"/>
    /// </summary>
    public class ReadOnlyCollectionGenericConverter : BaseGenericConverter
    {
        protected override Type ConvertibleType => typeof(ReadOnlyCollection<>);

        protected override object ConvertFromString(string str, Type type)
        {
            return Activator.CreateInstance(type, ConverterManager.Instance.ConvertFromString(str, typeof(List<>).MakeGenericType(type.GetGenericArguments())));
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ConverterManager.Instance.ConvertToString(obj, typeof(List<>).MakeGenericType(type.GetGenericArguments()));
        }
    }
}