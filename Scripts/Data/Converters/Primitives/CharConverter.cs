namespace UniT.Data
{
    using System;

    public sealed class CharConverter : PrimitiveConverter<char>
    {
        protected override object ConvertFromString(string str, Type type)
        {
            return char.Parse(str);
        }
    }
}