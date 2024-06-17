#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UnityEngine.Scripting;

    public sealed class UriConverter : Converter<Uri>
    {
        [Preserve]
        public UriConverter()
        {
        }

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