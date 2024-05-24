#nullable enable
namespace UniT.Data
{
    using System;

    public sealed class Int64Converter : PrimitiveConverter<long>
    {
        protected override object ConvertFromString(string str, Type type)
        {
            return long.Parse(str);
        }
    }
}