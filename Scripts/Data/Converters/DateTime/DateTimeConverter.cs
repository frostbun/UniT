namespace UniT.Data
{
    using System;
    using System.Globalization;

    public sealed class DateTimeConverter : Converter<DateTime>
    {
        private readonly string      format;
        private readonly CultureInfo cultureInfo;

        public DateTimeConverter(string format = "dd/MM/yyyy hh:mm:ss", CultureInfo cultureInfo = null)
        {
            this.format      = format;
            this.cultureInfo = cultureInfo ?? CultureInfo.InvariantCulture;
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return DateTime.ParseExact(str, this.format, this.cultureInfo);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ((DateTime)obj).ToString(this.format, this.cultureInfo);
        }
    }
}