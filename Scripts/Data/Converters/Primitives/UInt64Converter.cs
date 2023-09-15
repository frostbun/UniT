namespace UniT.Data.Converters.Primitives
{
    using System;

    public sealed class UInt64Converter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(ulong);

        protected override object ConvertFromString(string str, Type type)
        {
            return ulong.Parse(str);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}