#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using System.Globalization;
    using UnityEngine.Scripting;

    public sealed class DecimalConverter : Converter<Decimal>
    {
        private readonly NumberFormatInfo formatInfo;

        [Preserve]
        public DecimalConverter(NumberFormatInfo? formatInfo = null)
        {
            this.formatInfo = formatInfo ?? NumberFormatInfo.InvariantInfo;
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return Decimal.Parse(str, this.formatInfo);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ((Decimal)obj).ToString(this.formatInfo);
        }
    }
}