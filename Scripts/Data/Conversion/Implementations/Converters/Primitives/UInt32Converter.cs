#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UnityEngine.Scripting;

    public sealed class UInt32Converter : PrimitiveConverter<uint>
    {
        [Preserve]
        public UInt32Converter()
        {
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return uint.Parse(str);
        }
    }
}