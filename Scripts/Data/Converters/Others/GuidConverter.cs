namespace UniT.Data.Converters
{
    using System;

    public sealed class GuidConverter : BaseConverter
    {
        protected override Type ConvertibleType { get; } = typeof(Guid);

        protected override object ConvertFromString(string str, Type type)
        {
            return new Guid(str);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}