namespace UniT.Core.Converters.Implement
{
    using System;
    using UniT.Core.Converters.Base;
    using UniT.Core.Extensions;

    public class DoubleConverter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(double);

        protected override object ConvertFromString_Internal(string str, Type type)
        {
            return double.Parse(str.IsNullOrWhitespace() ? "0" : str);
        }

        protected override string ConvertToString_Internal(object obj, Type type)
        {
            return obj?.ToString() ?? "0";
        }
    }
}