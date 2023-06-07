namespace UniT.Core.Converters.Implement
{
    using System;
    using UniT.Core.Converters.Base;
    using UniT.Core.Extensions;

    public class Int32Converter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(int);

        protected override object ConvertFromString_Internal(string str, Type type)
        {
            return int.Parse(str.IsNullOrWhitespace() ? "0" : str);
        }

        protected override string ConvertToString_Internal(object obj, Type type)
        {
            return obj?.ToString() ?? "0";
        }
    }
}