#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using System.Globalization;
    using UnityEngine.Scripting;

    public sealed class Int16Converter : Converter<Int16>
    {
        private readonly NumberFormatInfo formatInfo;

        [Preserve]
        public Int16Converter(NumberFormatInfo? formatInfo = null)
        {
            this.formatInfo = formatInfo ?? NumberFormatInfo.InvariantInfo;
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return Int16.Parse(str, this.formatInfo);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ((Int16)obj).ToString(this.formatInfo);
        }
    }
}