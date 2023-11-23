namespace UniT.Data.Converters
{
    using System;

    public sealed class Int16Converter : BaseConverter
    {
        protected override Type ConvertibleType { get; } = typeof(short);

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