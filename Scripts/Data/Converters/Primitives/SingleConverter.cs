namespace UniT.Data.Converters
{
    using System;

    public sealed class SingleConverter : BaseConverter
    {
        protected override Type ConvertibleType { get; } = typeof(float);

        protected override object ConvertFromString(string str, Type type)
        {
            return float.Parse(str);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}