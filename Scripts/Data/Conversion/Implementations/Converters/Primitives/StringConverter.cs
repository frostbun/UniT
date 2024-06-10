#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UnityEngine.Scripting;

    public sealed class StringConverter : PrimitiveConverter<string>
    {
        [Preserve]
        public StringConverter()
        {
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return str;
        }
    }
}