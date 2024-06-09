#nullable enable
namespace UniT.Data.Conversion
{
    using System;

    public sealed class UInt16Converter : PrimitiveConverter<ushort>
    {
        protected override object ConvertFromString(string str, Type type)
        {
            return ushort.Parse(str);
        }
    }
}