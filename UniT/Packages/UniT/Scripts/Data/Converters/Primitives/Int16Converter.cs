namespace UniT.Data.Converters.Primitives
{
    using System;
    using UniT.Data.Converters.Base;

    public class Int16Converter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(short);

        protected override object ConvertFromString(string str, Type type)
        {
            return short.Parse(str);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}