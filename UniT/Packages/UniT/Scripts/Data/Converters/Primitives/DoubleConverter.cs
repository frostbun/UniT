namespace UniT.Data.Converters.Primitives
{
    using System;
    using UniT.Data.Converters.Base;

    public class DoubleConverter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(double);

        protected override object ConvertFromString_Internal(string str, Type type)
        {
            return double.Parse(str);
        }

        protected override string ConvertToString_Internal(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}