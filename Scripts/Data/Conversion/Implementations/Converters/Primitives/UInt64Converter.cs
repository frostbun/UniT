#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UnityEngine.Scripting;

    public sealed class UInt64Converter : PrimitiveConverter<ulong>
    {
        [Preserve]
        public UInt64Converter()
        {
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return ulong.Parse(str);
        }
    }
}