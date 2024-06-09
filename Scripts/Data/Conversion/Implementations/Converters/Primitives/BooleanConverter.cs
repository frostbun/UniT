#nullable enable
namespace UniT.Data.Conversion
{
    using System;

    public sealed class BooleanConverter : PrimitiveConverter<bool>
    {
        protected override object ConvertFromString(string str, Type type)
        {
            return bool.Parse(str);
        }
    }
}