#nullable enable
namespace UniT.Data.Conversion
{
    using System;

    public sealed class DecimalConverter : PrimitiveConverter<decimal>
    {
        protected override object ConvertFromString(string str, Type type)
        {
            return decimal.Parse(str);
        }
    }
}