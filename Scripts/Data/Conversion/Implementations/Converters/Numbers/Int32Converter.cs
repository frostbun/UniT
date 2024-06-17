#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using System.Globalization;
    using UnityEngine.Scripting;

    public sealed class Int32Converter : Converter<Int32>
    {
        private readonly NumberFormatInfo formatInfo;

        [Preserve]
        public Int32Converter(NumberFormatInfo? formatInfo = null)
        {
            this.formatInfo = formatInfo ?? NumberFormatInfo.InvariantInfo;
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return Int32.Parse(str, this.formatInfo);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ((Int32)obj).ToString(this.formatInfo);
        }
    }
}