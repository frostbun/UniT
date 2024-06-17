#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UnityEngine.Scripting;

    public sealed class StringConverter : Converter<String>
    {
        [Preserve]
        public StringConverter()
        {
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return str;
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}