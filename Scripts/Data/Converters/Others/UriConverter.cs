namespace UniT.Data.Converters.Others
{
    using System;

    public sealed class UriConverter : BaseConverter
    {
        protected override Type ConvertibleType => typeof(Uri);

        protected override object ConvertFromString(string str, Type type)
        {
            return new Uri(str);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}