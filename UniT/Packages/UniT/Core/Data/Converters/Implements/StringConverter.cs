namespace UniT.Core.Data.Converters.Implements
{
    using System;
    using UniT.Core.Data.Converters.Base;
    using UniT.Core.Extensions;

    public class StringConverter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(string);

        protected override object ConvertFromString_Internal(string str, Type type)
        {
            return str.IsNullOrWhitespace() ? string.Empty : str;
        }

        protected override string ConvertToString_Internal(object obj, Type type)
        {
            return obj?.ToString() ?? string.Empty;
        }
    }
}