#nullable enable
namespace UniT.Data
{
    using System;
    using System.Globalization;

    public sealed class DateTimeOffsetConverter : Converter<DateTimeOffset>
    {
        private readonly string      format;
        private readonly CultureInfo cultureInfo;

        public DateTimeOffsetConverter(string format = "dd/MM/yyyy hh:mm:ss", CultureInfo? cultureInfo = null)
        {
            this.format      = format;
            this.cultureInfo = cultureInfo ?? CultureInfo.InvariantCulture;
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return DateTimeOffset.ParseExact(str, this.format, this.cultureInfo);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ((DateTimeOffset)obj).ToString(this.format, this.cultureInfo);
        }
    }
}