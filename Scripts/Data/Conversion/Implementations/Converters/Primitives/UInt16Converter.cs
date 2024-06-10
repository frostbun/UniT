#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UnityEngine.Scripting;

    public sealed class UInt16Converter : PrimitiveConverter<ushort>
    {
        [Preserve]
        public UInt16Converter()
        {
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return ushort.Parse(str);
        }
    }
}