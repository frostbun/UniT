#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UnityEngine.Scripting;

    public sealed class EnumConverter : Converter<Enum>
    {
        [Preserve]
        public EnumConverter()
        {
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return Enum.Parse(type, str);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}