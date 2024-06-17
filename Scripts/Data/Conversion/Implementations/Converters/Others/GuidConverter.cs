#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UnityEngine.Scripting;

    public sealed class GuidConverter : Converter<Guid>
    {
        [Preserve]
        public GuidConverter()
        {
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return Guid.Parse(str);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}