#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UnityEngine.Scripting;

    public sealed class Int64Converter : PrimitiveConverter<long>
    {
        [Preserve]
        public Int64Converter()
        {
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return long.Parse(str);
        }
    }
}