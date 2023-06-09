namespace UniT.Core.Data.Converters.Implements
{
    using System;
    using UniT.Core.Data.Converters.Base;
    using UniT.Core.Extensions;

    public class BooleanConverter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(bool);

        protected override object ConvertFromString_Internal(string str, Type type)
        {
            return bool.Parse(str.IsNullOrWhitespace() ? "false" : str);
        }

        protected override string ConvertToString_Internal(object obj, Type type)
        {
            return obj?.ToString() ?? "false";
        }
    }
}