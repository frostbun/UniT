#nullable enable
namespace UniT.Data
{
    using System;

    public sealed class UInt64Converter : PrimitiveConverter<ulong>
    {
        protected override object ConvertFromString(string str, Type type)
        {
            return ulong.Parse(str);
        }
    }
}