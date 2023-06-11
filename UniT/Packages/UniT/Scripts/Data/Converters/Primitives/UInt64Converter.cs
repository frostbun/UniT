namespace UniT.Data.Converters.Primitives
{
    using System;
    using UniT.Data.Converters.Base;

    public class UInt64Converter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(ulong);

        protected override object ConvertFromString_Internal(string str, Type type)
        {
            return ulong.Parse(str);
        }

        protected override string ConvertToString_Internal(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}