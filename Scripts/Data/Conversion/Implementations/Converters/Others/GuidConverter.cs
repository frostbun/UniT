#nullable enable
namespace UniT.Data.Conversion
{
    using System;

    public sealed class GuidConverter : Converter<Guid>
    {
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