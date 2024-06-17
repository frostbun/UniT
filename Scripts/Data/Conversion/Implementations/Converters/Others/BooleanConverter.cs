#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UnityEngine.Scripting;

    public sealed class BooleanConverter : Converter<Boolean>
    {
        [Preserve]
        public BooleanConverter()
        {
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return Boolean.Parse(str);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}