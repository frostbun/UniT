namespace UniT.Data
{
    using System;

    public sealed class StringConverter : PrimitiveConverter<string>
    {
        protected override object ConvertFromString(string str, Type type)
        {
            return str;
        }
    }
}