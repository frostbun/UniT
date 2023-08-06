namespace UniT.Data.Converters.Primitives
{
    using System;

    public class Int64Converter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(long);

        protected override object ConvertFromString(string str, Type type)
        {
            return long.Parse(str);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}