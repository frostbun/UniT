#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using System.Globalization;
    using UnityEngine.Scripting;

    public sealed class DoubleConverter : Converter<Double>
    {
        private readonly NumberFormatInfo formatInfo;

        [Preserve]
        public DoubleConverter(NumberFormatInfo? formatInfo = null)
        {
            this.formatInfo = formatInfo ?? NumberFormatInfo.InvariantInfo;
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return Double.Parse(str, this.formatInfo);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ((Double)obj).ToString(this.formatInfo);
        }
    }
}