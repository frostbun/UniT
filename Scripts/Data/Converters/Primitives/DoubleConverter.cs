namespace UniT.Data.Converters
{
    using System;

    public sealed class DoubleConverter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(double);

        protected override object ConvertFromString(string str, Type type)
        {
            return double.Parse(str);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}