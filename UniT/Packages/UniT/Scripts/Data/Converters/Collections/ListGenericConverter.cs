namespace UniT.Data.Converters.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Data.Converters.Base;

    /// <summary>
    ///     Depends on <see cref="ArrayConverter"/>
    /// </summary>
    public class ListGenericConverter : BaseGenericConverter
    {
        protected override Type ConvertibleType => typeof(List<>);

        protected override object ConvertFromString(string str, Type type)
        {
            return Activator.CreateInstance(type, ConverterManager.Instance.ConvertFromString(str, type.GetGenericArguments()[0].MakeArrayType()));
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ConverterManager.Instance.ConvertToString(((IList)obj).Cast<object>().ToArray(), type.GetGenericArguments()[0].MakeArrayType());
        }
    }
}