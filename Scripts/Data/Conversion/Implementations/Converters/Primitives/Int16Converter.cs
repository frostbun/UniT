#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UnityEngine.Scripting;

    public sealed class Int16Converter : PrimitiveConverter<short>
    {
        [Preserve]
        public Int16Converter()
        {
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return short.Parse(str);
        }
    }
}