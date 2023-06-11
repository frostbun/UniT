namespace UniT.Data.Converters.Primitives
{
    using System;
    using UniT.Data.Converters.Base;

    public class DecimalConverter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(decimal);

        protected override object ConvertFromString_Internal(string str, Type type)
        {
            return decimal.Parse(str);
        }

        protected override string ConvertToString_Internal(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}