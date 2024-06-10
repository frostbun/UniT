#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UnityEngine.Scripting;

    public sealed class CharConverter : PrimitiveConverter<char>
    {
        [Preserve]
        public CharConverter()
        {
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return char.Parse(str);
        }
    }
}