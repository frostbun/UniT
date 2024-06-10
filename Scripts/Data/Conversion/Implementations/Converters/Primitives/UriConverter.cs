#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UnityEngine.Scripting;

    public sealed class UriConverter : PrimitiveConverter<Uri>
    {
        [Preserve]
        public UriConverter()
        {
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return new Uri(str);
        }
    }
}