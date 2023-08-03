namespace UniT.Data.Converters.Primitives
{
    using System;

    public class UInt16Converter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(ushort);

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