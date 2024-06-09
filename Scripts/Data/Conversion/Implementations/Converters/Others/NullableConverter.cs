#nullable enable
namespace UniT.Data.Conversion
{
    using System;
    using UniT.Extensions;

    public sealed class NullableConverter : Converter
    {
        protected override bool CanConvert(Type type) => type.IsGenericTypeOf(typeof(Nullable<>));

        protected override object ConvertFromString(string str, Type type)
        {
            return this.Manager.ConvertFromString(str, Nullable.GetUnderlyingType(type)!);
        }

        protected override string ConvertToString(object obj, Type type)
        {
            return this.Manager.ConvertToString(obj, Nullable.GetUnderlyingType(type)!);
        }
    }
}