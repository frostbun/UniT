#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using System.Globalization;
    using UnityEngine.Scripting;

    public sealed class DateTimeOffsetConverter : Converter<DateTimeOffset>
    {
        private readonly string             format;
        private readonly DateTimeFormatInfo formatInfo;

        [Preserve]
        public DateTimeOffsetConverter(string format = "dd/MM/yyyy hh:mm:ss", DateTimeFormatInfo? formatInfo = null)
        {
            this.format     = format;
            this.formatInfo = formatInfo ?? DateTimeFormatInfo.InvariantInfo;
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return DateTimeOffset.ParseExact(str, this.format, this.formatInfo);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ((DateTimeOffset)obj).ToString(this.format, this.formatInfo);
        }
    }
}