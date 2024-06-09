#nullable enable
namespace UniT.Data.Conversion
{
    using System;

    public sealed class Int16Converter : PrimitiveConverter<short>
    {
        protected override object ConvertFromString(string str, Type type)
        {
            return short.Parse(str);
        }
    }
}