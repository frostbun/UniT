namespace UniT.Data.Converters.Primitives
{
    using System;
    using UniT.Data.Converters.Base;

    public class UInt16Converter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(ushort);

        protected override object ConvertFromString_Internal(string str, Type type)
        {
            return ushort.Parse(str);
        }

        protected override string ConvertToString_Internal(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}