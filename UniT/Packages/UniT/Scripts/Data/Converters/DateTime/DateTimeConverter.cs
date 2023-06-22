namespace UniT.Data.Converters.DateTime
{
    using System;
    using System.Globalization;
    using UniT.Data.Converters.Base;

    public class DateTimeConverter : BaseConverter
    {
        private readonly string      format;
        private readonly CultureInfo cultureInfo;

        public DateTimeConverter(string format = "dd/MM/yyyy hh:mm:ss", CultureInfo cultureInfo = null)
        {
            this.format      = format;
            this.cultureInfo = cultureInfo ?? CultureInfo.InvariantCulture;
        }

        protected override Type ConvertibleType => typeof(DateTime);

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