#nullable enable
namespace UniT.Data
{
    using System;
    using UniT.Extensions;

    public class NullableConverter : Converter
    {
        protected override bool CanConvert(Type type) => type.DerivesFrom(typeof(Nullable<>));

        protected override object ConvertFromString(string str, Type type)
        {
            return ConverterManager.ConvertFromString(str, Nullable.GetUnderlyingType(type)!);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return ConverterManager.ConvertToString(obj, Nullable.GetUnderlyingType(type)!);
        }
    }
}