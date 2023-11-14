namespace UniT.Data.Converters
{
    using System;

    public sealed class Int32Converter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(int);

        protected override object ConvertFromString(string str, Type type)
        {
            return int.Parse(str);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}