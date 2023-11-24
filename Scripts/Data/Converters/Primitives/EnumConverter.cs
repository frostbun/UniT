namespace UniT.Data.Converters
{
    using System;

    public sealed class EnumConverter : PrimitiveConverter<Enum>
    {
        protected override object ConvertFromString(string str, Type type)
        {
            return Enum.Parse(type, str);
        }
    }
}