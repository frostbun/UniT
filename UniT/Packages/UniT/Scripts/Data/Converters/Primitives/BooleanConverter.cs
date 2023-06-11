namespace UniT.Data.Converters.Primitives
{
    using System;
    using UniT.Data.Converters.Base;

    public class BooleanConverter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(bool);

        protected override object ConvertFromString_Internal(string str, Type type)
        {
            return bool.Parse(str);
        }

        protected override string ConvertToString_Internal(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}