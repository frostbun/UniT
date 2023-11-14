namespace UniT.Data.Converters
{
    using System;

    public sealed class StringConverter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(string);

        protected override object ConvertFromString(string str, Type type)
        {
            return str;
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}