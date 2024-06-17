#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using System.Globalization;
    using UnityEngine.Scripting;

    public sealed class UInt32Converter : Converter<UInt32>
    {
        private readonly NumberFormatInfo formatInfo;

        [Preserve]
        public UInt32Converter(NumberFormatInfo? formatInfo = null)
        {
            this.formatInfo = formatInfo ?? NumberFormatInfo.InvariantInfo;
        }

        protected override object ConvertFromString(string str, Type type)
        {
            return UInt32.Parse(str, this.formatInfo);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ((UInt32)obj).ToString(this.formatInfo);
        }
    }
}