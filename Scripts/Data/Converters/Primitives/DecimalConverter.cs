namespace UniT.Data.Converters
{
    using System;

    public sealed class DecimalConverter : BaseConverter
    {
        protected override Type ConvertibleType { get; } = typeof(decimal);

        protected override object ConvertFromString(string str, Type type)
        {
            return decimal.Parse(str);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}