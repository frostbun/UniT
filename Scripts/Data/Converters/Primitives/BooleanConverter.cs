namespace UniT.Data.Converters.Primitives
{
    using System;

    public sealed class BooleanConverter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(bool);

        protected override object ConvertFromString(string str, Type type)
        {
            return bool.Parse(str);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}