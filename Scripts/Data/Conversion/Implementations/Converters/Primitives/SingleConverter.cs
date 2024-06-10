#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UnityEngine.Scripting;

    public sealed class SingleConverter : PrimitiveConverter<float>
    {
        [Preserve]
        public SingleConverter()
        {
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return float.Parse(str);
        }
    }
}