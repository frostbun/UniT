#nullable enable
namespace UniT.Data
{
    using System;

    public sealed class DoubleConverter : PrimitiveConverter<double>
    {
        protected override object ConvertFromString(string str, Type type)
        {
            return double.Parse(str);
        }
    }
}