namespace UniT.Data
{
    using System;

    public sealed class UInt32Converter : PrimitiveConverter<uint>
    {
        protected override object ConvertFromString(string str, Type type)
        {
            return uint.Parse(str);
        }
    }
}