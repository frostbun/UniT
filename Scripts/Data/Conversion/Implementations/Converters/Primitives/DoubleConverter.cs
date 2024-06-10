#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UnityEngine.Scripting;

    public sealed class DoubleConverter : PrimitiveConverter<double>
    {
        [Preserve]
        public DoubleConverter()
        {
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return double.Parse(str);
        }
    }
}