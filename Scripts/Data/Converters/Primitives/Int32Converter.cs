namespace UniT.Data.Converters
{
    using System;

    public sealed class Int32Converter : PrimitiveConverter<int>
    {
        protected override object ConvertFromString(string str, Type type)
        {
            return int.Parse(str);
        }
    }
}