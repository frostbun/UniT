#nullable enable
namespace UniT.Data.Conversion
{
    using System;

    public abstract class PrimitiveConverter<T> : Converter<T>
    {
        protected sealed override string ConvertToString(object obj, Type type)
        {
            return obj.ToString();
        }
    }
}