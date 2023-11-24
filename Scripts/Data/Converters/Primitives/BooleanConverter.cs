namespace UniT.Data.Converters
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