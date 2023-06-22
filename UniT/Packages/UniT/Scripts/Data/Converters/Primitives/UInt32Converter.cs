namespace UniT.Data.Converters.Primitives
{
    using System;
    using UniT.Data.Converters.Base;

    public class UInt32Converter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(uint);

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