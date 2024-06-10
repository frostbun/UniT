#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UnityEngine.Scripting;

    public sealed class BooleanConverter : PrimitiveConverter<bool>
    {
        [Preserve]
        public BooleanConverter()
        {
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return bool.Parse(str);
        }
    }
}