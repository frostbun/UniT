#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UnityEngine.Scripting;

    public sealed class GuidConverter : PrimitiveConverter<Guid>
    {
        [Preserve]
        public GuidConverter()
        {
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return new Guid(str);
        }
    }
}