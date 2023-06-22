namespace UniT.Data.Converters.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UniT.Data.Converters.Base;

    public class ListGenericConverter : BaseGenericConverter
    {
        protected override Type ConvertibleType => typeof(List<>);

        protected override object ConvertFromString(string str, Type type)
        {
            var arrayType      = type.GetGenericArguments()[0].MakeArrayType();
            var arrayConverter = ConverterManager.Instance.GetConverter(arrayType);
            return Activator.CreateInstance(type, arrayConverter.ConvertFromString(str, arrayType));
        }

        protected override string ConvertToString(object obj, Type type)
        {
            var arrayType      = type.GetGenericArguments()[0].MakeArrayType();
            var arrayConverter = ConverterManager.Instance.GetConverter(arrayType);
            return arrayConverter.ConvertToString(((IList)obj).Cast<object>().ToArray(), arrayType);
        }
    }
}