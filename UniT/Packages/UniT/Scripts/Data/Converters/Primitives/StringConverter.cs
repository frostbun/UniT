namespace UniT.Data.Converters.Primitives
{
    using System;
    using UniT.Data.Converters.Base;

    public class StringConverter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(string);

        protected override object ConvertFromString_Internal(string str, Type type)
        {
            return str;
        }

        protected override string ConvertToString_Internal(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}