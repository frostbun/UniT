namespace UniT.Data.Converters
{
    using System;
    using System.Globalization;

    public sealed class DateTimeOffsetConverter : BaseConverter
    {
        private readonly string      _format;
        private readonly CultureInfo _cultureInfo;

        public DateTimeOffsetConverter(string format = "dd/MM/yyyy hh:mm:ss", CultureInfo cultureInfo = null)
        {
            this._format      = format;
            this._cultureInfo = cultureInfo ?? CultureInfo.InvariantCulture;
        }

        protected override Type ConvertibleType => typeof(DateTime);

        protected override object ConvertFromString(string str, Type type)
        {
            return DateTimeOffset.ParseExact(str, this._format, this._cultureInfo);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ((DateTimeOffset)obj).ToString(this._format, this._cultureInfo);
        }
    }
}