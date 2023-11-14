namespace UniT.Data.Converters
{
    using System;

    public sealed class EnumConverter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(Enum);

        protected override object ConvertFromString(string str, Type type)
        {
            return Enum.Parse(type, str);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}