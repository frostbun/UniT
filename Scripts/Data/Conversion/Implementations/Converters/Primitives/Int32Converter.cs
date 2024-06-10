#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UnityEngine.Scripting;

    public sealed class Int32Converter : PrimitiveConverter<int>
    {
        [Preserve]
        public Int32Converter()
        {
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return int.Parse(str);
        }
    }
}