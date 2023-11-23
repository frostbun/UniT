namespace UniT.Data.Converters
{
    using System;

    public sealed class UInt32Converter : BaseConverter
    {
        protected override Type ConvertibleType { get; } = typeof(uint);

        protected override object ConvertFromString(string str, Type type)
        {
            return uint.Parse(str);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}