namespace UniT.Data.Converters.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     Depends on <see cref="ArrayConverter"/>
    /// </summary>
    public sealed class ListConverter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(List<>);

        protected override object ConvertFromString(string str, Type type)
        {
            return Activator.CreateInstance(type, ConverterManager.Instance.ConvertFromString(str, MakeArrayType(type)));
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ConverterManager.Instance.ConvertToString(((IList)obj).Cast<object>().ToArray(), MakeArrayType(type));
        }

        private static Type MakeArrayType(Type type)
        {
            return type.GetGenericArguments()[0].MakeArrayType();
        }
    }
}