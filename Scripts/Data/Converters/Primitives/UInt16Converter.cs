namespace UniT.Data.Converters
{
    using System;

    public sealed class UInt16Converter : BaseConverter
    {
        protected override Type ConvertibleType { get; } = typeof(ushort);

        protected override object ConvertFromString(string str, Type type)
        {
            return ushort.Parse(str);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}