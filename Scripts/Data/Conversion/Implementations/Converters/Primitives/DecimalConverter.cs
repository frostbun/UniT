#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UnityEngine.Scripting;

    public sealed class DecimalConverter : PrimitiveConverter<decimal>
    {
        [Preserve]
        public DecimalConverter()
        {
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return decimal.Parse(str);
        }
    }
}