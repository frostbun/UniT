namespace UniT.Data.Converters
{
    using System;
    using System.Globalization;

    public sealed class DateTimeConverter : BaseConverter
    {
        private readonly string      _format;
        private readonly CultureInfo _cultureInfo;

        public DateTimeConverter(string format = "dd/MM/yyyy hh:mm:ss", CultureInfo cultureInfo = null)
        {
            this._format      = format;
            this._cultureInfo = cultureInfo ?? CultureInfo.InvariantCulture;
        }

        protected override Type ConvertibleType => typeof(DateTime);

        protected override object ConvertFromString(string str, Type type)
        {
            return DateTime.ParseExact(str, this._format, this._cultureInfo);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ((DateTime)obj).ToString(this._format, this._cultureInfo);
        }
    }
}