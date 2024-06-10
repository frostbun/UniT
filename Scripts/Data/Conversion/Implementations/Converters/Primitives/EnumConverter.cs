#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UnityEngine.Scripting;

    public sealed class EnumConverter : PrimitiveConverter<Enum>
    {
        [Preserve]
        public EnumConverter()
        {
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return Enum.Parse(type, str);
        }
    }
}