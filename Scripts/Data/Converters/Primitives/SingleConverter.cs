namespace UniT.Data
{
    using System;

    public sealed class SingleConverter : PrimitiveConverter<float>
    {
        protected override object ConvertFromString(string str, Type type)
        {
            return float.Parse(str);
        }
    }
}