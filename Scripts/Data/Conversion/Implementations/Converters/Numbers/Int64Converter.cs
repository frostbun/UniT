#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using System.Globalization;
    using UnityEngine.Scripting;

    public sealed class Int64Converter : Converter<Int64>
    {
        private readonly NumberFormatInfo formatInfo;

        [Preserve]
        public Int64Converter(NumberFormatInfo? formatInfo = null)
        {
            this.formatInfo = formatInfo ?? NumberFormatInfo.InvariantInfo;
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return Int64.Parse(str, this.formatInfo);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ((Int64)obj).ToString(this.formatInfo);
        }
    }
}