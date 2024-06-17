#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UnityEngine.Scripting;

    public sealed class CharConverter : Converter<Char>
    {
        [Preserve]
        public CharConverter()
        {
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return Char.Parse(str);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}